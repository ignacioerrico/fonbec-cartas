namespace Fonbec.Cartas.DataAccess.Entities
{
    public abstract class Auditable
    {
        public DateTimeOffset CreatedOnUtc { get; set; } = DateTimeOffset.UtcNow;
        
        public DateTimeOffset? LastUpdatedOnUtc { get; set; }

        public DateTimeOffset? SoftDeletedOnUtc { get; set; }

        public bool IsDeleted => SoftDeletedOnUtc.HasValue;
    }
}
