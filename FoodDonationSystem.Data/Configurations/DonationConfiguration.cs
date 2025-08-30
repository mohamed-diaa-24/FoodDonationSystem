using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FoodType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.Description)
                   .HasMaxLength(500);

            builder.Property(e => e.SpecialInstructions)
                   .HasMaxLength(300);

            builder.Property(e => e.ContactPerson)
                   .HasMaxLength(100);

            builder.Property(e => e.ContactPhone)
                   .HasMaxLength(20);

            // Relationships
            builder.HasOne(d => d.Restaurant)
                   .WithMany(r => r.Donations)
                   .HasForeignKey(d => d.RestaurantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Images)
                   .WithOne(i => i.Donation)
                   .HasForeignKey(i => i.DonationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Reservations)
                   .WithOne(r => r.Donation)
                   .HasForeignKey(r => r.DonationId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
