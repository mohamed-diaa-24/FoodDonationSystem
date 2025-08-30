using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    public class VolunteerAreaConfiguration : IEntityTypeConfiguration<VolunteerArea>
    {
        public void Configure(EntityTypeBuilder<VolunteerArea> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.AreaName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.CenterLatitude)
                   .HasPrecision(10, 8);

            builder.Property(e => e.CenterLongitude)
                   .HasPrecision(11, 8);

            // Relationships
            builder.HasOne(a => a.Volunteer)
                   .WithMany(v => v.ServiceAreas)
                   .HasForeignKey(a => a.VolunteerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
