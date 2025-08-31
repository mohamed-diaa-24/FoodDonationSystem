using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.Description)
                   .HasMaxLength(500);

            builder.Property(e => e.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Latitude)
                   .HasPrecision(10, 8);

            builder.Property(e => e.Longitude)
                   .HasPrecision(11, 8);

            builder.Property(e => e.LicenseDocument)
                   .HasMaxLength(500);

            builder.Property(e => e.CommercialRegister)
                   .HasMaxLength(500);

            builder.Property(e => e.RejectionReason)
                   .HasMaxLength(500);

            // Relationships - Updated to use ApplicationUser
            builder.HasOne(r => r.User)
                   .WithOne(u => u.Restaurant)
                   .HasForeignKey<Restaurant>(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Donations)
                   .WithOne(d => d.Restaurant)
                   .HasForeignKey(d => d.RestaurantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Schedules)
                   .WithOne(s => s.Restaurant)
                   .HasForeignKey(s => s.RestaurantId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
