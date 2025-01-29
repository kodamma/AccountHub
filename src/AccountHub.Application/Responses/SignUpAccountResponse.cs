namespace AccountHub.Application.Responses
{
    public class SignUpAccountResponse
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
