namespace Rystem.Background
{
    public class BackgroundJobOptions
    {
        public string Key { get; set; }
        public bool RunImmediately { get; set; }
        public string Cron { get; set; }
    }
}