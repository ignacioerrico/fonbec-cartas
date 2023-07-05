using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fonbec.Cartas.DataAccess.Configuration
{
    public abstract class UserWithAccountConfiguration<T> : IEntityTypeConfiguration<T>
        where T : UserWithAccount
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.FirstName);

            builder.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.LastName);

            builder.Property(t => t.NickName)
                .HasMaxLength(MaxLength.Actor.NickName);

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.Email);

            builder.Property(t => t.Phone)
                .HasMaxLength(MaxLength.Actor.Phone);

            builder.Property(t => t.AspNetUserId)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(MaxLength.Actor.AspNetUserId);

            builder.Property(t => t.Username)
                .IsRequired()
                .HasMaxLength(MaxLength.Actor.Username);

            builder.HasQueryFilter(t => !t.SoftDeletedOnUtc.HasValue);
        }
    }
}
