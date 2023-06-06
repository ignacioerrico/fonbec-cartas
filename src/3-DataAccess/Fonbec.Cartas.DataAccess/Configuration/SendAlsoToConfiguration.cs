using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class SendAlsoToConfiguration : IEntityTypeConfiguration<SendAlsoTo>
    {
        public void Configure(EntityTypeBuilder<SendAlsoTo> builder)
        {
            builder.HasKey(sat => sat.Id);

            builder.Property(sat => sat.RecipientFullName)
                .IsRequired()
                .HasMaxLength(MaxLength.SendAlsoTo.FullName);

            builder.Property(sat => sat.RecipientEmail)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.Email);
        }
    }
}
