using System.Text.Json;
using System.Text.RegularExpressions;
using HighTech.Abstraction;
using HighTech.Configurator;
using HighTech.DTOs;
using HighTech.DTOs.Assistant;
using HighTech.Options;
using HighTech.Services.Assistant;
using HighTech.Services.Assistant.Providers;
using Microsoft.Extensions.Options;

namespace HighTech.Services
{
    public class AssistantService : IAssistantService
    {
        private readonly IAiChatProvider provider;
        private readonly AssistantOptions options;
        private readonly AssistantDataResolver dataResolver;
        private readonly IAssistantConversationStore conversationStore;
        private readonly IConfiguratorService configurator;
        private readonly ILogger<AssistantService> logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public AssistantService(
            IAiChatProvider provider,
            IOptions<AssistantOptions> options,
            AssistantDataResolver dataResolver,
            IAssistantConversationStore conversationStore,
            IConfiguratorService configurator,
            ILogger<AssistantService> logger)
        {
            this.provider = provider;
            this.options = options.Value;
            this.dataResolver = dataResolver;
            this.conversationStore = conversationStore;
            this.configurator = configurator;
            this.logger = logger;
        }

        public async Task<AssistantChatResponseDTO> ChatAsync(AssistantChatRequestDTO request, string userId, string sessionId, CancellationToken ct = default)
        {
            request.Context ??= new AssistantContextDTO();
            request.Context.CurrentSelection ??= new Dictionary<string, string>();

            var history = conversationStore.GetHistory(userId, sessionId);
            var userTurnContent = SerializeUserTurn(request.Message, request.Context, fetchedData: null);

            AssistantMessageDTO answer;

            try
            {
                var classification = await ClassifyAsync(history, userTurnContent, ct);
                answer = await ProduceAnswerAsync(request, history, classification, userId, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Assistant chat failed for user {UserId}", userId);
                answer = new AssistantChatMessageDTO
                {
                    Text = "Sorry, I couldn't process that just now. Please try again.",
                    Rationale = AssistantRationales.InternalError
                };
            }

            var assistantContent = JsonSerializer.Serialize<AssistantMessageDTO>(answer, JsonOptions);
            conversationStore.Append(
                userId,
                sessionId,
                new AiChatMessage("user", userTurnContent),
                new AiChatMessage("assistant", assistantContent));

            return new AssistantChatResponseDTO
            {
                Message = answer,
                Products = await ResolveProductInfoAsync(answer, ct)
            };
        }

        private async Task<AssistantMessageDTO> ProduceAnswerAsync(
            AssistantChatRequestDTO request,
            IReadOnlyList<AiChatMessage> history,
            AssistantClassificationDTO classification,
            string userId,
            CancellationToken ct)
        {
            if (string.Equals(classification.Intent, AssistantIntents.Other, StringComparison.OrdinalIgnoreCase))
                return new AssistantChatMessageDTO
                {
                    Text = "I can only help with PC building, components, and tech questions about the catalog.",
                    Rationale = AssistantRationales.OutOfScope
                };

            if (!request.Context.OnConfigurator && IsBuildIntended(classification.Intent))
                return new AssistantChatMessageDTO
                {
                    Text = "Open the PC Configurator first, then I can build a draft for you.",
                    Rationale = AssistantRationales.OffConfigurator
                };

            var fetched = await ResolveFetchedDataAsync(classification, request, history, userId, ct);
            var ctx = new AnswerContext(history, request.Message, request.Context, fetched);

            var answer = await AnswerAsync(ctx, classification.Intent, ct);
            return await ResolveCompatibleAnswerAsync(ctx, answer, ct);
        }

        private async Task<AssistantFetchedDataDTO> ResolveFetchedDataAsync(
            AssistantClassificationDTO classification,
            AssistantChatRequestDTO request,
            IReadOnlyList<AiChatMessage> history,
            string userId,
            CancellationToken ct)
        {
            if (!(classification.DataPlan?.Fetches?.Count > 0))
                return null;

            var plan = classification.DataPlan!;

            try
            {
                var fetched = await dataResolver.ResolveAsync(plan, request.Context, BuildProductReferenceTexts(request.Message, history), ct);

                if (IsBuildIntended(classification.Intent) && fetched.Products.Values.All(list => list.Count == 0))
                {
                    logger.LogInformation(
                        "Assistant fetched 0 products for build intent. Plan categories: {Categories}",
                        string.Join(", ", plan.Fetches.Select(f => f.CategoryName ?? "<null>")));
                }

                return fetched;
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Assistant data plan rejected for user {UserId}", userId);
                return new AssistantFetchedDataDTO();
            }
        }

        private static bool IsBuildIntended(string intent) =>
            string.Equals(intent, AssistantIntents.PcCreation, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(intent, AssistantIntents.PcModification, StringComparison.OrdinalIgnoreCase);

        private static bool IsBuildAnswer(AssistantMessageDTO answer) =>
            answer is AssistantPcCreationMessageDTO or AssistantPcModificationMessageDTO;

        private sealed record AnswerContext(
            IReadOnlyList<AiChatMessage> History,
            string UserMessage,
            AssistantContextDTO Context,
            AssistantFetchedDataDTO FetchedData);

        private async Task<Dictionary<string, AssistantProductSummaryDTO>> ResolveProductInfoAsync(
            AssistantMessageDTO answer,
            CancellationToken ct)
        {
            IEnumerable<string> ids;
            switch (answer)
            {
                case AssistantPcCreationMessageDTO creation:
                    ids = creation.Selection?.Values ?? Enumerable.Empty<string>();
                    break;
                case AssistantPcModificationMessageDTO modification:
                    ids = modification.Changes?.Select(c => c.ProductId) ?? Enumerable.Empty<string>();
                    break;
                default:
                    return new Dictionary<string, AssistantProductSummaryDTO>();
            }

            try
            {
                return await dataResolver.GetProductSummariesAsync(ids, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to resolve product summaries for assistant response.");
                return new Dictionary<string, AssistantProductSummaryDTO>();
            }
        }

        private async Task<AssistantClassificationDTO> ClassifyAsync(
            IReadOnlyList<AiChatMessage> history,
            string userTurnContent,
            CancellationToken ct)
        {
            var messages = new List<AiChatMessage>(history.Count + 2)
            {
                new("system", AssistantPrompts.Classification())
            };
            messages.AddRange(history);
            messages.Add(new AiChatMessage("user", userTurnContent));

            var json = await provider.CompleteJsonAsync(
                new AiChatRequest(messages, AssistantSchemas.DataPlanSchema, "data_plan", options.Model),
                ct);

            var classification = JsonSerializer.Deserialize<AssistantClassificationDTO>(json, JsonOptions);
            return classification ?? new AssistantClassificationDTO { Intent = AssistantIntents.Other };
        }

        private async Task<AssistantMessageDTO> AnswerAsync(
            AnswerContext ctx,
            string intent,
            CancellationToken ct)
        {
            var userTurnContent = SerializeUserTurn(ctx.UserMessage, ctx.Context, ctx.FetchedData, intent);

            var messages = new List<AiChatMessage>(ctx.History.Count + 2)
            {
                new("system", AssistantPrompts.Answer())
            };
            messages.AddRange(ctx.History);
            messages.Add(new AiChatMessage("user", userTurnContent));

            var json = await provider.CompleteJsonAsync(
                new AiChatRequest(messages, AssistantSchemas.AssistantMessageSchema, "assistant_message", options.Model),
                ct);

            return ParseAnswer(json);
        }

        private const int MaxFixAttempts = 3;

        private async Task<AssistantMessageDTO> ResolveCompatibleAnswerAsync(
            AnswerContext ctx,
            AssistantMessageDTO answer,
            CancellationToken ct)
        {
            if (!IsBuildAnswer(answer))
                return answer;

            var fixableIssues = GetFixableIssues(answer, ctx.Context);
            if (fixableIssues.Count == 0)
                return answer;

            var current = answer;

            for (var attempt = 1; attempt <= MaxFixAttempts; attempt++)
            {
                AssistantMessageDTO fixedAnswer;
                try
                {
                    fixedAnswer = await FixAsync(ctx, current, fixableIssues, ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Assistant fix call failed on attempt {Attempt}.", attempt);
                    return new AssistantChatMessageDTO
                    {
                        Text = "I had trouble producing a compatible build. Please try again.",
                        Rationale = AssistantRationales.FixFailed
                    };
                }

                // Fix LLM gave up and returned a chat reply — pass it through.
                if (!IsBuildAnswer(fixedAnswer))
                    return fixedAnswer;

                var remaining = GetFixableIssues(fixedAnswer, ctx.Context);
                if (remaining.Count == 0)
                    return fixedAnswer;

                logger.LogInformation(
                    "Assistant fix attempt {Attempt}/{Max} still invalid ({IssueCount} fixable issues remain).",
                    attempt, MaxFixAttempts, remaining.Count);

                current = fixedAnswer;
                fixableIssues = remaining;
            }

            AssistantMessageDTO fallback = TryBuildPcCreationFromCatalog(ctx.History, ctx.UserMessage, ctx.FetchedData);
            return fallback ?? BuildValidationFailedMessage(fixableIssues);
        }

        private List<IncompatibilityDTO> GetFixableIssues(AssistantMessageDTO answer, AssistantContextDTO context)
        {
            var validation = ValidateAnswer(answer, context);
            return FilterFixableIssues(validation?.Issues);
        }

        private static AssistantChatMessageDTO BuildValidationFailedMessage(IReadOnlyCollection<IncompatibilityDTO> remainingIssues)
        {
            var issueLines = remainingIssues
                .Select(i => i.Message)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Take(4)
                .ToList();

            var detail = issueLines.Count > 0
                ? "Remaining issues:\n- " + string.Join("\n- ", issueLines)
                : "No specific issues were reported.";

            return new AssistantChatMessageDTO
            {
                Text = $"I couldn't find a fully compatible build in the catalog. {detail}\n\nThis usually means the catalog doesn't have a part that satisfies the constraint — a different brand or socket might not even exist as an option. Try removing one of the conflicting parts manually, or pick a different starting CPU/Motherboard.",
                Rationale = AssistantRationales.ValidationFailedAfterFix
            };
        }

        private AssistantPcCreationMessageDTO TryBuildPcCreationFromCatalog(
            IReadOnlyList<AiChatMessage> history,
            string userMessage,
            AssistantFetchedDataDTO fetchedData)
        {
            if (fetchedData?.Products == null || fetchedData.Products.Count == 0)
                return null;

            var budget = ExtractBudget(userMessage, history);
            var preferences = BuildPreferences.From(userMessage, history);

            if (!TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.Cpu, out var cpus) ||
                !TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.Motherboard, out var motherboards) ||
                !TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.Ram, out var rams) ||
                !TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.Gpu, out var gpus) ||
                !TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.Storage, out var storages) ||
                !TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.Psu, out var psus) ||
                !TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.Case, out var cases) ||
                !TryGetProducts(fetchedData, PcConfiguratorConstants.Categories.CpuCooler, out var coolers))
                return null;

            cpus = ApplyCpuPreferences(cpus, preferences)
                .OrderByDescending(p => CpuPreferenceScore(p, preferences))
                .ThenByDescending(p => p.Price)
                .ToList();

            gpus = gpus
                .OrderByDescending(p => GpuPreferenceScore(p, budget))
                .ThenByDescending(p => p.Price)
                .ToList();

            BuildCandidate best = null;

            foreach (var cpu in cpus)
            {
                var cpuSocket = Field(cpu, PcConfiguratorConstants.Fields.SocketType);
                if (string.IsNullOrWhiteSpace(cpuSocket)) continue;

                var matchingMotherboards = motherboards
                    .Where(m => Same(Field(m, PcConfiguratorConstants.Fields.SocketType), cpuSocket))
                    .OrderByDescending(m => m.Price);

                foreach (var motherboard in matchingMotherboards)
                {
                    var ramType = Field(motherboard, PcConfiguratorConstants.Fields.RamType);
                    var motherboardFormFactor = Field(motherboard, PcConfiguratorConstants.Fields.MotherboardFormFactor);
                    if (string.IsNullOrWhiteSpace(ramType) || string.IsNullOrWhiteSpace(motherboardFormFactor)) continue;

                    var ram = rams
                        .Where(r => Same(Field(r, PcConfiguratorConstants.Fields.RamType), ramType))
                        .Where(r => RamSpeedFits(r, motherboard))
                        .OrderByDescending(r => r.Price)
                        .FirstOrDefault();
                    if (ram == null) continue;

                    var storage = storages
                        .Where(s => StorageFits(s, motherboard))
                        .OrderByDescending(s => s.Price)
                        .FirstOrDefault();
                    if (storage == null) continue;

                    var matchingCases = cases
                        .Where(c => ContainsToken(Field(c, PcConfiguratorConstants.Fields.SupportedMotherboardFormFactors), motherboardFormFactor))
                        .OrderByDescending(c => c.Price);

                    foreach (var pcCase in matchingCases)
                    {
                        var cooler = coolers
                            .Where(c => ContainsToken(Field(c, PcConfiguratorConstants.Fields.SocketType), cpuSocket))
                            .Where(c => IntField(c, PcConfiguratorConstants.Fields.HeightMm) <= IntField(pcCase, PcConfiguratorConstants.Fields.MaxCpuCoolerHeightMm))
                            .OrderByDescending(c => c.Price)
                            .FirstOrDefault();
                        if (cooler == null) continue;

                        foreach (var gpu in gpus)
                        {
                            if (IntField(gpu, PcConfiguratorConstants.Fields.LengthMm) > IntField(pcCase, PcConfiguratorConstants.Fields.MaxGpuLengthMm))
                                continue;

                            var recommendedPsu = IntField(gpu, PcConfiguratorConstants.Fields.RecommendedPsuW);
                            var psu = psus
                                .Where(p => IntField(p, PcConfiguratorConstants.Fields.WattageW) >= recommendedPsu)
                                .OrderByDescending(p => p.Price)
                                .FirstOrDefault();
                            if (psu == null) continue;

                            var candidate = new BuildCandidate(cpu, motherboard, ram, gpu, storage, psu, pcCase, cooler);
                            if (budget.HasValue && candidate.Total > budget.Value * 1.08m)
                                continue;

                            if (best != null && candidate.Score(budget) <= best.Score(budget))
                                continue;

                            var candidateValidation = configurator.Validate(candidate.ToSelection());
                            if (HasRuleViolation(candidateValidation))
                                continue;

                            best = candidate;
                        }
                    }
                }
            }

            if (best == null)
                return null;

            var selection = best.ToSelection();
            var budgetText = budget.HasValue ? $" around EUR {budget.Value:0}" : " from the available catalog";
            return new AssistantPcCreationMessageDTO
            {
                Selection = selection,
                Rationale = $"Built a compatible configuration{budgetText}, prioritizing requested CPU family/socket preferences and stronger parts before cheap filler."
            };
        }

        private static bool TryGetProducts(AssistantFetchedDataDTO fetchedData, string categoryName, out List<AssistantProductDTO> products)
        {
            products = null;
            if (fetchedData.Products == null ||
                !fetchedData.Products.TryGetValue(categoryName, out var raw) ||
                raw == null ||
                raw.Count == 0)
                return false;

            products = raw;
            return true;
        }

        private static IEnumerable<AssistantProductDTO> ApplyCpuPreferences(IEnumerable<AssistantProductDTO> cpus, BuildPreferences preferences)
        {
            var filtered = cpus;

            if (!string.IsNullOrWhiteSpace(preferences.Socket))
            {
                var socketMatches = filtered.Where(p => Same(Field(p, PcConfiguratorConstants.Fields.SocketType), preferences.Socket)).ToList();
                if (socketMatches.Count > 0) filtered = socketMatches;
            }

            if (!string.IsNullOrWhiteSpace(preferences.CpuFamily))
            {
                var familyMatches = filtered.Where(p => p.Model?.IndexOf(preferences.CpuFamily, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                if (familyMatches.Count > 0) filtered = familyMatches;
            }

            return filtered;
        }

        private static decimal? ExtractBudget(string userMessage, IReadOnlyList<AiChatMessage> history)
        {
            var texts = new List<string> { userMessage ?? string.Empty };
            texts.AddRange(history
                .Where(m => string.Equals(m.Role, "user", StringComparison.OrdinalIgnoreCase))
                .Reverse()
                .Take(8)
                .Select(m => ExtractStoredUserMessage(m.Content)));

            foreach (var text in texts)
            {
                var match = Regex.Match(text, @"(?<!\d)([1-9]\d{2,4})(?:\s*(?:eur|euro|€))?", RegexOptions.IgnoreCase);
                if (match.Success && decimal.TryParse(match.Groups[1].Value, out var value) && value >= 300)
                    return value;
            }

            return null;
        }

        private static int CpuPreferenceScore(AssistantProductDTO product, BuildPreferences preferences)
        {
            var score = 0;
            if (!string.IsNullOrWhiteSpace(preferences.CpuFamily) &&
                product.Model?.IndexOf(preferences.CpuFamily, StringComparison.OrdinalIgnoreCase) >= 0)
                score += 100;
            if (!string.IsNullOrWhiteSpace(preferences.Socket) &&
                Same(Field(product, PcConfiguratorConstants.Fields.SocketType), preferences.Socket))
                score += 50;
            if (product.Model?.IndexOf("Ryzen 5", StringComparison.OrdinalIgnoreCase) >= 0 ||
                product.Model?.IndexOf("Core i5", StringComparison.OrdinalIgnoreCase) >= 0)
                score += 20;
            return score;
        }

        private static decimal GpuPreferenceScore(AssistantProductDTO product, decimal? budget)
        {
            if (!budget.HasValue)
                return product.Price;

            var target = budget.Value * 0.45m;
            var distance = Math.Abs(product.Price - target);
            return Math.Max(0, target - distance);
        }

        private static bool StorageFits(AssistantProductDTO storage, AssistantProductDTO motherboard)
        {
            var storageInterface = Field(storage, PcConfiguratorConstants.Fields.StorageInterface);
            if (string.IsNullOrWhiteSpace(storageInterface)) return false;

            if (storageInterface.IndexOf("NVMe", StringComparison.OrdinalIgnoreCase) >= 0)
                return IntField(motherboard, PcConfiguratorConstants.Fields.M2Slots) > 0;

            if (storageInterface.IndexOf("SATA", StringComparison.OrdinalIgnoreCase) >= 0)
                return IntField(motherboard, PcConfiguratorConstants.Fields.SataPorts) > 0;

            return true;
        }

        private static bool RamSpeedFits(AssistantProductDTO ram, AssistantProductDTO motherboard)
        {
            var moboMax = IntField(motherboard, PcConfiguratorConstants.Fields.MaxRamSpeedMhz);
            if (moboMax <= 0) return true;
            var ramSpeed = IntField(ram, PcConfiguratorConstants.Fields.RamSpeedMhz);
            return ramSpeed <= moboMax;
        }

        // True only when validation flags an actual compatibility-rule violation.
        // AssemblyService and CompleteBuild issues are catalog/data gaps, not bad picks —
        // they make IsValid false but shouldn't disqualify an otherwise compatible build.
        private static bool HasRuleViolation(ConfiguratorValidationResultDTO validation)
        {
            if (validation?.Issues == null) return false;

            foreach (var issue in validation.Issues)
            {
                if (issue == null) continue;
                if (string.Equals(issue.Rule, PcConfiguratorConstants.RuleCodes.AssemblyService, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (string.Equals(issue.Rule, PcConfiguratorConstants.RuleCodes.CompleteBuild, StringComparison.OrdinalIgnoreCase))
                    continue;
                return true;
            }
            return false;
        }

        private static string Field(AssistantProductDTO product, string fieldName)
        {
            return product?.Fields?
                .FirstOrDefault(f => string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase))
                ?.Value;
        }

        private static int IntField(AssistantProductDTO product, string fieldName)
        {
            var value = Field(product, fieldName);
            return int.TryParse(value, out var parsed) ? parsed : 0;
        }

        private static bool Same(string left, string right) =>
            string.Equals(left?.Trim(), right?.Trim(), StringComparison.OrdinalIgnoreCase);

        private static bool ContainsToken(string raw, string token)
        {
            if (string.IsNullOrWhiteSpace(raw) || string.IsNullOrWhiteSpace(token)) return false;

            return raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Any(part => string.Equals(part, token, StringComparison.OrdinalIgnoreCase));
        }

        private sealed record BuildCandidate(
            AssistantProductDTO Cpu,
            AssistantProductDTO Motherboard,
            AssistantProductDTO Ram,
            AssistantProductDTO Gpu,
            AssistantProductDTO Storage,
            AssistantProductDTO Psu,
            AssistantProductDTO Case,
            AssistantProductDTO Cooler)
        {
            public decimal Total => Cpu.Price + Motherboard.Price + Ram.Price + Gpu.Price + Storage.Price + Psu.Price + Case.Price + Cooler.Price;

            public decimal Score(decimal? budget)
            {
                if (!budget.HasValue)
                    return Total;

                var target = budget.Value * 0.97m;
                var distancePenalty = Math.Abs(Total - target);
                return Total - distancePenalty;
            }

            public Dictionary<string, string> ToSelection() => new(StringComparer.OrdinalIgnoreCase)
            {
                [PcConfiguratorConstants.Categories.Cpu] = Cpu.Id,
                [PcConfiguratorConstants.Categories.Motherboard] = Motherboard.Id,
                [PcConfiguratorConstants.Categories.Ram] = Ram.Id,
                [PcConfiguratorConstants.Categories.Gpu] = Gpu.Id,
                [PcConfiguratorConstants.Categories.Storage] = Storage.Id,
                [PcConfiguratorConstants.Categories.Psu] = Psu.Id,
                [PcConfiguratorConstants.Categories.Case] = Case.Id,
                [PcConfiguratorConstants.Categories.CpuCooler] = Cooler.Id
            };
        }

        private sealed class BuildPreferences
        {
            public string Socket { get; init; }
            public string CpuFamily { get; init; }

            public static BuildPreferences From(string userMessage, IReadOnlyList<AiChatMessage> history)
            {
                var combined = string.Join("\n", history
                    .Where(m => string.Equals(m.Role, "user", StringComparison.OrdinalIgnoreCase))
                    .Reverse()
                    .Take(6)
                    .Select(m => ExtractStoredUserMessage(m.Content))
                    .Append(userMessage ?? string.Empty));

                return new BuildPreferences
                {
                    Socket = LatestMatch(combined, "AM4", "AM5", "LGA1700", "LGA1200"),
                    CpuFamily = LatestMatch(combined, "Ryzen 5", "Core i5", "i5")
                        ?.Replace("i5", "Core i5", StringComparison.OrdinalIgnoreCase)
                };
            }

            private static string LatestMatch(string text, params string[] values)
            {
                return values
                    .Select(value => new { Value = value, Index = text.LastIndexOf(value, StringComparison.OrdinalIgnoreCase) })
                    .Where(match => match.Index >= 0)
                    .OrderByDescending(match => match.Index)
                    .FirstOrDefault()
                    ?.Value;
            }
        }

        private static string ExtractStoredUserMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("message", out var message) &&
                    message.ValueKind == JsonValueKind.String)
                    return message.GetString() ?? string.Empty;
            }
            catch (JsonException)
            {
                // Older history entries or tests may store plain text.
            }

            return content;
        }

        // The Fix loop should only react to real compatibility issues (R1..R9). Two other kinds
        // of issues come out of Validate but the assistant can't act on them:
        // - CompleteBuild: "Select a X to complete the PC configuration." — the user simply hasn't
        //   picked that part yet. Not a constraint violation. The Build Summary's missing-parts UI
        //   already nudges the user.
        // - missing field: a Product is missing a required spec field — a catalog data gap the
        //   admin should fix, not something the assistant can resolve by swapping parts.
        // - AssemblyService: the seed for the Assembly product is absent — admin / seeding concern.
        private static List<IncompatibilityDTO> FilterFixableIssues(ICollection<IncompatibilityDTO> issues)
        {
            if (issues == null || issues.Count == 0)
                return new List<IncompatibilityDTO>();

            return issues
                .Where(i => !string.IsNullOrWhiteSpace(i?.Message))
                .Where(i => !string.Equals(i.Rule, PcConfiguratorConstants.RuleCodes.CompleteBuild, StringComparison.OrdinalIgnoreCase))
                .Where(i => !string.Equals(i.Rule, PcConfiguratorConstants.RuleCodes.AssemblyService, StringComparison.OrdinalIgnoreCase))
                .Where(i => i.Message.IndexOf("missing field", StringComparison.OrdinalIgnoreCase) < 0)
                .ToList();
        }

        private ConfiguratorValidationResultDTO ValidateAnswer(AssistantMessageDTO answer, AssistantContextDTO context)
        {
            var merged = MergeIntoSelection(answer, context?.CurrentSelection);
            if (merged == null) return null;

            try
            {
                return configurator.Validate(merged);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Configurator.Validate threw while validating assistant build.");
                return null;
            }
        }

        private static IDictionary<string, string> MergeIntoSelection(
            AssistantMessageDTO answer,
            IDictionary<string, string> currentSelection)
        {
            switch (answer)
            {
                case AssistantPcCreationMessageDTO creation:
                    return new Dictionary<string, string>(creation.Selection ?? new(), StringComparer.OrdinalIgnoreCase);

                case AssistantPcModificationMessageDTO modification:
                    {
                        var merged = new Dictionary<string, string>(
                            currentSelection ?? new Dictionary<string, string>(),
                            StringComparer.OrdinalIgnoreCase);

                        if (modification.Changes != null)
                        {
                            foreach (var change in modification.Changes)
                            {
                                if (string.IsNullOrWhiteSpace(change.CategoryName)) continue;

                                if (string.IsNullOrEmpty(change.ProductId))
                                    merged.Remove(change.CategoryName);
                                else
                                    merged[change.CategoryName] = change.ProductId;
                            }
                        }
                        return merged;
                    }

                default:
                    return null;
            }
        }

        private async Task<AssistantMessageDTO> FixAsync(
            AnswerContext ctx,
            AssistantMessageDTO previousAttempt,
            IReadOnlyCollection<IncompatibilityDTO> issues,
            CancellationToken ct)
        {
            var previousAttemptJson = JsonSerializer.Serialize<AssistantMessageDTO>(previousAttempt, JsonOptions);
            var userTurnContent = SerializeFixTurn(ctx.UserMessage, ctx.Context, ctx.FetchedData, previousAttemptJson, issues);

            var messages = new List<AiChatMessage>(ctx.History.Count + 2)
            {
                new("system", AssistantPrompts.Fix())
            };
            messages.AddRange(ctx.History);
            messages.Add(new AiChatMessage("user", userTurnContent));

            var json = await provider.CompleteJsonAsync(
                new AiChatRequest(messages, AssistantSchemas.AssistantMessageSchema, "assistant_message", options.Model),
                ct);

            return ParseAnswer(json);
        }

        private static string SerializeFixTurn(
            string message,
            AssistantContextDTO context,
            AssistantFetchedDataDTO fetchedData,
            string previousAttemptJson,
            IReadOnlyCollection<IncompatibilityDTO> issues)
        {
            var payload = new Dictionary<string, object>
            {
                ["message"] = message,
                ["context"] = new
                {
                    onConfigurator = context.OnConfigurator,
                    currentSelection = context.CurrentSelection ?? new Dictionary<string, string>()
                },
                ["fetchedData"] = fetchedData ?? new AssistantFetchedDataDTO(),
                ["previousAttempt"] = JsonSerializer.Deserialize<JsonElement>(previousAttemptJson),
                ["incompatibilities"] = (issues ?? Array.Empty<IncompatibilityDTO>())
                    .Select(i => new
                    {
                        rule = i.Rule,
                        categoryA = i.CategoryA,
                        categoryB = i.CategoryB,
                        message = i.Message
                    })
                    .ToList()
            };

            return JsonSerializer.Serialize(payload, JsonOptions);
        }

        private static AssistantMessageDTO ParseAnswer(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string type = root.TryGetProperty("type", out var typeEl) ? typeEl.GetString() : null;
            string rationale = root.TryGetProperty("rationale", out var ratEl) && ratEl.ValueKind == JsonValueKind.String
                ? ratEl.GetString()
                : null;

            switch (type)
            {
                case "chat":
                    {
                        var text = root.TryGetProperty("text", out var t) && t.ValueKind == JsonValueKind.String
                            ? t.GetString()
                            : string.Empty;
                        return new AssistantChatMessageDTO { Text = text, Rationale = rationale };
                    }

                case "pc-creation":
                    {
                        var selection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        if (root.TryGetProperty("selection", out var sel) && sel.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in sel.EnumerateArray())
                            {
                                if (item.ValueKind != JsonValueKind.Object) continue;

                                var rawCategory = item.TryGetProperty("categoryName", out var cn) && cn.ValueKind == JsonValueKind.String
                                    ? cn.GetString()
                                    : null;
                                var productId = item.TryGetProperty("productId", out var pid) && pid.ValueKind == JsonValueKind.String
                                    ? pid.GetString()
                                    : null;

                                var category = AssistantDataResolver.NormalizeCategory(rawCategory) ?? rawCategory;

                                if (!string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(productId))
                                    selection[category] = productId;
                            }
                        }
                        return new AssistantPcCreationMessageDTO { Selection = selection, Rationale = rationale };
                    }

                case "pc-modification":
                    {
                        var changes = new List<AssistantPcModificationChangeDTO>();
                        if (root.TryGetProperty("changes", out var ch) && ch.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in ch.EnumerateArray())
                            {
                                if (item.ValueKind != JsonValueKind.Object) continue;

                                var rawCategory = item.TryGetProperty("categoryName", out var cn) && cn.ValueKind == JsonValueKind.String
                                    ? cn.GetString()
                                    : null;
                                var productId = item.TryGetProperty("productId", out var pid) && pid.ValueKind == JsonValueKind.String
                                    ? pid.GetString()
                                    : null;

                                var category = AssistantDataResolver.NormalizeCategory(rawCategory) ?? rawCategory;

                                if (!string.IsNullOrWhiteSpace(category))
                                {
                                    changes.Add(new AssistantPcModificationChangeDTO
                                    {
                                        CategoryName = category,
                                        ProductId = productId
                                    });
                                }
                            }
                        }
                        return new AssistantPcModificationMessageDTO { Changes = changes, Rationale = rationale };
                    }

                default:
                    return new AssistantChatMessageDTO
                    {
                        Text = "Sorry, I couldn't compose a useful answer.",
                        Rationale = AssistantRationales.UnrecognizedType
                    };
            }
        }

        private static string SerializeUserTurn(string message, AssistantContextDTO context, AssistantFetchedDataDTO fetchedData, string intent = null)
        {
            var payload = new Dictionary<string, object>
            {
                ["message"] = message,
                ["context"] = new
                {
                    onConfigurator = context.OnConfigurator,
                    currentSelection = context.CurrentSelection ?? new Dictionary<string, string>()
                }
            };

            if (intent != null)
                payload["intent"] = intent;

            if (fetchedData != null)
                payload["fetchedData"] = fetchedData;

            return JsonSerializer.Serialize(payload, JsonOptions);
        }

        private static IEnumerable<string> BuildProductReferenceTexts(string message, IReadOnlyList<AiChatMessage> history)
        {
            if (!string.IsNullOrWhiteSpace(message))
                yield return message;

            foreach (var item in history ?? Array.Empty<AiChatMessage>())
            {
                if (!string.IsNullOrWhiteSpace(item.Content))
                    yield return item.Content;
            }
        }

    }
}
