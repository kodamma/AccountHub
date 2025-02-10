namespace AccountHub.Application.Responses
{
    public class SignUpAccountResponse
    {
        public required Guid AccountId { get; set; } 
        public required string Username { get; set; }
    }
}
