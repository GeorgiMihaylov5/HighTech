namespace HighTech.Options
{
    public class AssistantOptions
    {
        public string Provider { get; set; }
        public string Model { get; set; }

        public string ApiKey { get; set; }

        public int MaxHistoryMessages { get; set; } = 20;
        public int RateLimitPerUserPerHour { get; set; } = 30;
        public int MaxFetchesPerRequest { get; set; } = 9;
        public int MaxProductsPerFetch { get; set; } = 30;
        public int ConversationIdleTimeoutMinutes { get; set; } = 5;
    }
}
