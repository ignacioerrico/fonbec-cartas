using Fonbec.Cartas.DataAccess.Constants;
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

            builder.Property(pe => pe.Subject)
                .IsRequired()
                .HasMaxLength(MaxLength.Plan.Subject);

            builder.Property(pe => pe.MessageMarkdown)
                .IsRequired()
                .HasMaxLength(MaxLength.Plan.MessageMarkdown);

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
