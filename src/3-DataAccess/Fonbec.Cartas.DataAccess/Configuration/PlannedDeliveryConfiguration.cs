using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class PlannedDeliveryConfiguration : IEntityTypeConfiguration<PlannedDelivery>
    {
        public void Configure(EntityTypeBuilder<PlannedDelivery> builder)
        {
            builder.HasKey(pd => pd.Id);

            builder.HasOne(pd => pd.PlannedEvent)
                .WithMany(pe => pe.PlannedDeliveries)
                .HasForeignKey(pd => pd.PlannedEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pd => pd.Apadrinamiento)
                .WithMany()
                .HasForeignKey(pd => pd.ApadrinamientoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pd => pd.DeliveryApprovedByRevisor)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(pd => pd.DeliveryApprovedByRevisorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(pd => !pd.Apadrinamiento.SoftDeletedOnUtc.HasValue);
        }
    }
}
