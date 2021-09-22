namespace Rystem.Background
{
    public interface IBackgroundOptionedJob : IBackgroundJob
    {
        BackgroundJobOptions Options { get; }
    }
}