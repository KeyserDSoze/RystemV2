namespace System
{
    public sealed class StopwatchStart
    {
        public DateTime Start { get; } = DateTime.UtcNow;
        public StopwatchResult Stop() 
            => new(Start, DateTime.UtcNow);
    }
}