using AccountHub.Domain.Base;

namespace AccountHub.Domain.Entities
{
    public class RefreshToken : Entity
    {
        public Guid AccountId { get; set; }
        public string Hash { get; set; } = null!;
        public DateTimeOffset Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTimeOffset? Revoked { get; set; } 
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
