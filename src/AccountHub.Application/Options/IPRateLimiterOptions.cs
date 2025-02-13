namespace AccountHub.Application.Options
{
    public class IpRateLimiterOptions
    {
        public const string Name = "RateLimiter";
        public int MaxAttempts { get; set; }
        public int Window { get; set; }
        public int QueueLimit { get; set; }
        public int BlockTimeMinutes { get; set; }
    }
}
