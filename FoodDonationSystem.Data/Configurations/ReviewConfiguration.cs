using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Rating)
                   .IsRequired();

            builder.Property(e => e.Comment)
                   .HasMaxLength(500);

            // Check constraint for rating
            builder.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5");

            // Relationships - Updated to use ApplicationUser
            builder.HasOne(r => r.FromUser)
                   .WithMany(u => u.GivenReviews)
                   .HasForeignKey(r => r.FromUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.ToUser)
                   .WithMany(u => u.ReceivedReviews)
                   .HasForeignKey(r => r.ToUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Reservation)
                   .WithMany()
                   .HasForeignKey(r => r.ReservationId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);

        }
    }
}
