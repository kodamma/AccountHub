namespace AccountHub.Domain.Base
{
    public abstract class Entity
    {
        protected Entity() => Id = Guid.NewGuid();
        protected Entity(Guid id) => Id = id;
        public Guid Id { get; private init; }
        public DateTimeOffset CreatedAt { get; private init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    }
}
