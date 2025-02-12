namespace AccountHub.Application.Options
{
    public class IpRateLimitingOptions
    {
        public const string Name = "RateLimiter";
        public int PermitLimit { get; set; }
        public TimeSpan Window { get; set; }
        public int QueueLimit { get; set; }
    }
}
