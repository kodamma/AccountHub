using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AccountHub.Application.Options
{
    public class JwtOptions
    {
        public const string Name = "JWTOptions";
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Key { get; set; } = null!;
        public int LifeTime { get; set; }
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
            => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
}
