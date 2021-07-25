namespace Rystem.Background
{
    public class BackgroundWorkOptions
    {
        public string Key { get; set; }
        public bool RunImmediately { get; set; }
        public string Cron { get; set; }
    }
}