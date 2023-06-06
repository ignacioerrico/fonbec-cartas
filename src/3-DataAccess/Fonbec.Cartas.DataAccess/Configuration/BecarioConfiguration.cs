using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class BecarioConfiguration : IEntityTypeConfiguration<Becario>
    {
        public void Configure(EntityTypeBuilder<Becario> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.FirstName)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.FirstName);

            builder.Property(b => b.LastName)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.LastName);

            builder.Property(b => b.NickName)
                .HasMaxLength(MaxLength.Actor.NickName);

            builder.Property(b => b.Email)
                .HasMaxLength(MaxLength.Actor.Email);

            builder.Property(b => b.Phone)
                .HasMaxLength(MaxLength.Actor.Phone);

            builder.Property(b => b.BecarioGuid)
                .HasDefaultValueSql("NEWID()");

            builder.HasOne(b => b.Mediador)
                .WithMany(m => m.Becarios)
                .HasForeignKey(b => b.MediadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.CreatedByCoordinador)
                .WithMany()
                .HasForeignKey(b => b.CreatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.UpdatedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(b => b.UpdatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.DeletedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(b => b.DeletedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(b => !b.SoftDeletedOnUtc.HasValue);
        }
    }
}
