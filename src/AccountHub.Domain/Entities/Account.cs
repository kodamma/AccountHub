using AccountHub.Domain.Base;

namespace AccountHub.Domain.Entities
{
    public class Account : Entity
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public required DateOnly Birthdate { get; set; }
        public string? AvatarURL { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public bool Locked { get; set; } = false;
        public int LockedCount { get; set; }
        public DateTimeOffset LockedEnd { get; set; }
    }
}
