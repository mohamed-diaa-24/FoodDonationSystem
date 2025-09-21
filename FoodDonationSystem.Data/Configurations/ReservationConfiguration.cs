using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Notes)
                   .HasMaxLength(300);

            builder.Property(e => e.PickupPersonName)
                   .HasMaxLength(100);

            builder.Property(e => e.PickupPersonPhone)
                   .HasMaxLength(20);

            // Relationships
            builder.HasOne(r => r.Donation)
                   .WithMany(d => d.Reservations)
                   .HasForeignKey(r => r.DonationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Charity)
                   .WithMany(c => c.Reservations)
                   .HasForeignKey(r => r.CharityId)
                   .OnDelete(DeleteBehavior.NoAction);


            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
