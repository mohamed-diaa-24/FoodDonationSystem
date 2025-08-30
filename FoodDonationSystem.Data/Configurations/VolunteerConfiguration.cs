using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
    {
        public void Configure(EntityTypeBuilder<Volunteer> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.VehicleType)
                   .HasMaxLength(50);

            builder.Property(e => e.VehicleNumber)
                   .HasMaxLength(20);

            builder.Property(e => e.DriverLicense)
                   .HasMaxLength(500);

            builder.Property(e => e.AverageRating)
                   .HasPrecision(3, 2);

            // Relationships
            builder.HasOne(v => v.User)
                   .WithOne(u => u.Volunteer)
                   .HasForeignKey<Volunteer>(v => v.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.ServiceAreas)
                   .WithOne(a => a.Volunteer)
                   .HasForeignKey(a => a.VolunteerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.Deliveries)
                   .WithOne(d => d.Volunteer)
                   .HasForeignKey(d => d.VolunteerId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
