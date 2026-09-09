namespace HighTech.DTOs.Assistant
{
    public class AssistantClassificationDTO
    {
        public string Intent { get; set; }
        public string Reasoning { get; set; }
        public AssistantDataPlanDTO DataPlan { get; set; } = new();
    }

    public static class AssistantIntents
    {
        public const string TechQuestion = "tech_question";
        public const string PcCreation = "pc_creation";
        public const string PcModification = "pc_modification";
        public const string Other = "other";
    }

    public static class AssistantRationales
    {
        public const string OutOfScope = "out_of_scope";
        public const string OffConfigurator = "off_configurator";
        public const string InternalError = "internal_error";
        public const string FixFailed = "fix_failed";
        public const string ValidationFailedAfterFix = "validation_failed_after_fix";
        public const string UnrecognizedType = "unrecognized_type";
    }
}
