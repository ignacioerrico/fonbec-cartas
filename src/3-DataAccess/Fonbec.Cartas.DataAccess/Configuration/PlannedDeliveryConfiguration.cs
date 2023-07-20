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

            builder.HasOne(pd => pd.FromBecario)
                .WithMany()
                .HasForeignKey(pd => pd.FromBecarioId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pd => pd.ToPadrino)
                .WithMany()
                .HasForeignKey(pd => pd.ToPadrinoId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
