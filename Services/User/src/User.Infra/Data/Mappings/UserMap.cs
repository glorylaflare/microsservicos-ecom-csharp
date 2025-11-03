using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.Infra.Data.Mappings;

internal class UserMap : IEntityTypeConfiguration<Domain.Models.User>
{
    public void Configure(EntityTypeBuilder<Domain.Models.User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(20);
        builder.Property(u => u.Auth0UserId).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Auth0UserId).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(o => o.CreatedAt).IsRequired().HasColumnType("datetime");
        builder.Property(o => o.UpdatedAt).HasColumnType("datetime");
    }
}
