namespace HighTech.Abstraction
{
    public interface IAssistantRateLimiter
    {
        bool TryConsume(string userId);
    }
}
