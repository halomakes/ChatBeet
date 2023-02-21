namespace ChatBeet.Exceptions;

public class KarmaRateLimitException : Exception
{
    public TimeSpan Delay { get; }
    
    public KarmaRateLimitException(TimeSpan delay) : base($"You must wait {delay} to change this karma level again.")
    {
        Delay = delay;
    }
}