using System.Text.Json.Serialization;

namespace AccountHub.Application.Responses
{
    public class AuthResponse
    {
        public required Guid AccountId { get; set; }
        public required string AccessToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; } = null!;
    }
}
