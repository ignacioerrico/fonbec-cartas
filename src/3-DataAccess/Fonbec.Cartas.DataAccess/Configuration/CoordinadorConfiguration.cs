using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class CoordinadorConfiguration : IEntityTypeConfiguration<Coordinador>
    {
        public void Configure(EntityTypeBuilder<Coordinador> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.FirstName);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.LastName);

            builder.Property(c => c.NickName)
                .HasMaxLength(MaxLength.Actor.NickName);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.Email);

            builder.Property(c => c.Phone)
                .HasMaxLength(MaxLength.Actor.Phone);

            builder.Property(c => c.AspNetUserId)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(MaxLength.Actor.AspNetUserId);

            builder.Property(c => c.Username)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.Username);

            builder.HasQueryFilter(f => !f.SoftDeletedOnUtc.HasValue);
        }
    }
}
