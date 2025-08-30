using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class RestaurantScheduleConfiguration : IEntityTypeConfiguration<RestaurantSchedule>
    {
        public void Configure(EntityTypeBuilder<RestaurantSchedule> builder)
        {
            builder.HasKey(e => e.Id);

            // Relationships
            builder.HasOne(s => s.Restaurant)
                   .WithMany(r => r.Schedules)
                   .HasForeignKey(s => s.RestaurantId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one schedule per day per restaurant
            builder.HasIndex(s => new { s.RestaurantId, s.DayOfWeek })
                   .IsUnique();
        }
    }
}
