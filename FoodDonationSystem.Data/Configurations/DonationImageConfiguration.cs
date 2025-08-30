using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class DonationImageConfiguration
    {
        public void Configure(EntityTypeBuilder<DonationImage> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ImagePath)
                   .IsRequired()
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(i => i.Donation)
                   .WithMany(d => d.Images)
                   .HasForeignKey(i => i.DonationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
