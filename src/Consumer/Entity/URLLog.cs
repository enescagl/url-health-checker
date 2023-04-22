namespace URLHealthChecker.Consumer.Entity
{
    public class URLLog
    {
        public string CallingService { get; set; } = null!;
        public string URL { get; set; } = null!;
        public string StatusCode { get; set; } = null!;
    }
}