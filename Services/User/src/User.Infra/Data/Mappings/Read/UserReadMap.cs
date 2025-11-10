using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.Infra.Data.Mappings.Read;

internal class UserReadMap : IEntityTypeConfiguration<UserReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.ToView("vw_Users");
        builder.HasNoKey();

        builder.Property(u => u.Id);
        builder.Property(u => u.Username);
        builder.Property(u => u.Email);
        builder.Property(o => o.CreatedAt);
        builder.Property(o => o.UpdatedAt);
    }
}
