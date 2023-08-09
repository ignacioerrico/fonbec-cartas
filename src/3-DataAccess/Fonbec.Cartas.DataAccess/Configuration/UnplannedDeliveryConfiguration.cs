using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class UnplannedDeliveryConfiguration : IEntityTypeConfiguration<UnplannedDelivery>
    {
        public void Configure(EntityTypeBuilder<UnplannedDelivery> builder)
        {
            builder.HasKey(ud => ud.Id);

            builder.HasOne(ud => ud.Deadline)
                .WithMany(d => d.UnplannedDeliveries)
                .HasForeignKey(ud => ud.DeadlineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ud => ud.Apadrinamiento)
                .WithMany()
                .HasForeignKey(ud => ud.ApadrinamientoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ud => ud.DeliveryApprovedByRevisor)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(ud => ud.DeliveryApprovedByRevisorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(ud => !ud.Apadrinamiento.SoftDeletedOnUtc.HasValue);
        }
    }
}
