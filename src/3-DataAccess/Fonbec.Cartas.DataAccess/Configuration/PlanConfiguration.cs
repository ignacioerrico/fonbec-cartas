using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Subject)
                .IsRequired()
                .HasMaxLength(MaxLength.Plan.Subject);

            builder.Property(p => p.MessageMarkdown)
                .IsRequired()
                .HasMaxLength(MaxLength.Plan.MessageMarkdown);

            builder.HasOne(p => p.Filial)
                .WithMany()
                .HasForeignKey(p => p.FilialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.CreatedByCoordinador)
                .WithMany()
                .HasForeignKey(p => p.CreatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.UpdatedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(p => p.UpdatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.DeletedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(p => p.DeletedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(p => !p.SoftDeletedOnUtc.HasValue);
        }
    }
}
