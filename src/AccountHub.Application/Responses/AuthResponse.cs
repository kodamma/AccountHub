using System.Text.Json.Serialization;

namespace AccountHub.Application.Responses
{
    public class AuthResponse
    {
        public string AccountId { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        [JsonIgnore]
        public string RefreshToken { get; set; } = null!;
    }
}
