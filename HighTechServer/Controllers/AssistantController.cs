using System.Security.Claims;
using HighTech.Abstraction;
using HighTech.Common;
using HighTech.DTOs.Assistant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class AssistantController : Controller
    {
        private readonly IAssistantService assistantService;
        private readonly IAssistantRateLimiter rateLimiter;
        private readonly IAssistantConversationStore conversationStore;
        private readonly ILogger<AssistantController> logger;

        public AssistantController(
            IAssistantService assistantService,
            IAssistantRateLimiter rateLimiter,
            IAssistantConversationStore conversationStore,
            ILogger<AssistantController> logger)
        {
            this.assistantService = assistantService;
            this.rateLimiter = rateLimiter;
            this.conversationStore = conversationStore;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] AssistantChatRequestDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                return Response.ResultFailure<AssistantChatResponseDTO>("Message is required.", 400);

            if (string.IsNullOrWhiteSpace(dto.SessionId))
                return Response.ResultFailure<AssistantChatResponseDTO>("SessionId is required.", 400);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Response.ResultFailure<AssistantChatResponseDTO>("Unauthorized.", 401);

            if (!rateLimiter.TryConsume(userId))
                return Response.ResultFailure<AssistantChatResponseDTO>("Rate limit exceeded. Try again later.", 429);

            try
            {
                var response = await assistantService.ChatAsync(dto, userId, dto.SessionId, HttpContext.RequestAborted);
                return Response.ResultSuccess(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception in Assistant/Chat for user {UserId}", userId);
                return Response.ResultFailure<AssistantChatResponseDTO>("An unexpected error occurred.", 500);
            }
        }

        [HttpPost]
        public IActionResult NewChat([FromBody] NewChatRequestDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Response.ResultFailure<object>("Unauthorized.", 401);

            if (dto != null && !string.IsNullOrWhiteSpace(dto.SessionId))
                conversationStore.Clear(userId, dto.SessionId);

            return Response.ResultSuccess<object>(null);
        }
    }
}
