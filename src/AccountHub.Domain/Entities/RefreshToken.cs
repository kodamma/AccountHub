using AccountHub.Domain.Base;

namespace AccountHub.Domain.Entities
{
    public class RefreshToken : Entity
    {
        public Guid AccountId { get; set; }
        public string Hash { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
        public bool Revoked { get; set; } = false;
    }
}
