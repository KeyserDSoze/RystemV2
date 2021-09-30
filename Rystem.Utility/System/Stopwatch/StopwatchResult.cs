namespace System
{
    public record StopwatchResult(DateTime Start, DateTime Stop)
    {
        public TimeSpan Span => Stop - Start;
    }
}