namespace AccountHub.Application.Options
{
    public class RabbitMqOptions
    {
        public const string Name = "RabbitMq";
        public string Host {  get; set; }
        public string Username {  get; set; }
        public string Password {  get; set; }
        public int RetryCount { get; set; }
        public int Interval { get; set; }
    }
}
