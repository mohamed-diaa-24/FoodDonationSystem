using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class CharityNeedConfiguration : IEntityTypeConfiguration<CharityNeed>
    {
        public void Configure(EntityTypeBuilder<CharityNeed> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FoodType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.Description)
                   .HasMaxLength(300);

            // Relationships
            builder.HasOne(n => n.Charity)
                   .WithMany(c => c.Needs)
                   .HasForeignKey(n => n.CharityId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
