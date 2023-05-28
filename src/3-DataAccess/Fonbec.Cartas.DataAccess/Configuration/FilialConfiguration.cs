using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class FilialConfiguration : IEntityTypeConfiguration<Filial>
    {
        public void Configure(EntityTypeBuilder<Filial> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(MaxLength.Filial.Name);

            builder.HasMany(f => f.Coordinadores)
                .WithOne(c => c.Filial)
                .HasForeignKey(c => c.FilialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Mediadores)
                .WithOne(c => c.Filial)
                .HasForeignKey(c => c.FilialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Revisores)
                .WithOne(c => c.Filial)
                .HasForeignKey(c => c.FilialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(f => !f.SoftDeletedOnUtc.HasValue);
        }
    }
}
