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

            builder.HasMany(p => p.SendAlsoTo)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.Phone)
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

            builder.HasQueryFilter(f => !f.SoftDeletedOnUtc.HasValue);
        }
    }
}
