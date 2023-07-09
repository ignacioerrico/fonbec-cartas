using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class PadrinoConfiguration : IEntityTypeConfiguration<Padrino>
    {
        public void Configure(EntityTypeBuilder<Padrino> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.FirstName);

            builder.Property(p => p.LastName)
                .HasMaxLength(MaxLength.Actor.LastName);

            builder.Property(p => p.NickName)
                .HasMaxLength(MaxLength.Actor.NickName);

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.Email);

            builder.HasMany(p => p.SendAlsoTo)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Phone)
                .HasMaxLength(MaxLength.Actor.Phone);

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
