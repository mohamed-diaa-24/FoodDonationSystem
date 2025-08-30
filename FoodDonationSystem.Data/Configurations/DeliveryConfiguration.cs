using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.DeliveryNotes)
                   .HasMaxLength(300);

            builder.Property(e => e.ProofOfDelivery)
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(d => d.Reservation)
                   .WithOne(r => r.Delivery)
                   .HasForeignKey<Delivery>(d => d.ReservationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Volunteer)
                   .WithMany(v => v.Deliveries)
                   .HasForeignKey(d => d.VolunteerId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
