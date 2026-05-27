using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Players;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlayerProfileConfiguration : IEntityTypeConfiguration<PlayerProfile>
{
    public void Configure(EntityTypeBuilder<PlayerProfile> builder)
    {
        builder.ToTable("player_profiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id)
            .HasColumnName("id");

        builder.Property(profile => profile.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(profile => profile.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(profile => profile.UserId)
            .IsUnique()
            .HasDatabaseName("ux_player_profiles_user_id");

        builder.HasMany(profile => profile.Civilizations)
            .WithOne()
            .HasForeignKey(civilization => civilization.PlayerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(profile => profile.Civilizations)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
