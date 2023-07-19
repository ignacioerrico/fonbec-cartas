using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class CartaObligatoriaConfiguration : IEntityTypeConfiguration<CartaObligatoria>
    {
        public void Configure(EntityTypeBuilder<CartaObligatoria> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Subject)
                .IsRequired()
                .HasMaxLength(MaxLength.Plan.Subject);

            builder.Property(p => p.MessageMarkdown)
                .IsRequired()
                .HasMaxLength(MaxLength.Plan.MessageMarkdown);
        }
    }
}
