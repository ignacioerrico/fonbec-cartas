using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class PlannedEventConfiguration : IEntityTypeConfiguration<PlannedEvent>
    {
        public void Configure(EntityTypeBuilder<PlannedEvent> builder)
        {
            builder.HasKey(pe => pe.Id);

            builder.HasOne(pe => pe.Filial)
                .WithMany()
                .HasForeignKey(pe => pe.FilialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pe => pe.CartaObligatoria)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<PlannedEvent>(pe => pe.CartaObligatoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pe => pe.CreatedByCoordinador)
                .WithMany()
                .HasForeignKey(pe => pe.CreatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pe => pe.UpdatedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(pe => pe.UpdatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pe => pe.DeletedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(pe => pe.DeletedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(pe => !pe.SoftDeletedOnUtc.HasValue);
        }
    }
}
