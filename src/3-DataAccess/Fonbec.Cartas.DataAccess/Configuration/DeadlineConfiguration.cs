using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class DeadlineConfiguration : IEntityTypeConfiguration<Deadline>
    {
        public void Configure(EntityTypeBuilder<Deadline> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.Filial)
                .WithMany()
                .HasForeignKey(d => d.FilialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.CreatedByCoordinador)
                .WithMany()
                .HasForeignKey(d => d.CreatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.UpdatedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(d => d.UpdatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.DeletedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(d => d.DeletedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(d => !d.SoftDeletedOnUtc.HasValue);
        }
    }
}
