using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class CharityImageConfiguration : IEntityTypeConfiguration<CharityImage>
    {
        public void Configure(EntityTypeBuilder<CharityImage> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ImagePath)
                   .IsRequired()
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(i => i.Charity)
                   .WithMany(c => c.Images)
                   .HasForeignKey(i => i.CharityId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}

