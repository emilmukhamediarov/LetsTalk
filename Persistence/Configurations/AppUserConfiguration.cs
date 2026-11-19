using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LetsTalk.Domain.Entities;

namespace LetsTalk.Persistence.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.UserName)
                 .HasMaxLength(20);

            builder.HasIndex(u => u.UserName)
                 .IsUnique();

            builder.Property(u => u.Email)
                .HasMaxLength(256);

            builder.HasIndex(u => u.Email)
                .IsUnique();

        }
    }
}
