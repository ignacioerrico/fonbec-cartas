﻿using Fonbec.Cartas.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public class ApadrinamientoConfiguration : IEntityTypeConfiguration<Apadrinamiento>
    {
        public void Configure(EntityTypeBuilder<Apadrinamiento> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Becario)
                .WithMany(b => b.Apadrinamientos)
                .HasForeignKey(a => a.BecarioId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasOne(a => a.Padrino)
                .WithMany(p => p.Apadrinamientos)
                .HasForeignKey(a => a.PadrinoId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(b => b.ApadrinamientoGuid)
                .HasDefaultValueSql("NEWID()");

            builder.HasOne(a => a.CreatedByCoordinador)
                .WithMany()
                .HasForeignKey(a => a.CreatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.UpdatedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(a => a.UpdatedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.DeletedByCoordinador)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(a => a.DeletedByCoordinadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(a => !a.SoftDeletedOnUtc.HasValue);
        }
    }
}
