using FoodDonationSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDonationSystem.Data.Configurations
{
    internal class CharityConfiguration : IEntityTypeConfiguration<Charity>
    {
        public void Configure(EntityTypeBuilder<Charity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.Description)
                   .HasMaxLength(500);

            builder.Property(e => e.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Latitude)
                   .HasPrecision(10, 8);

            builder.Property(e => e.Longitude)
                   .HasPrecision(11, 8);

            builder.Property(e => e.LicenseDocument)
                   .HasMaxLength(500);

            builder.Property(e => e.ProofDocument)
                   .HasMaxLength(500);

            builder.Property(e => e.RejectionReason)
                   .HasMaxLength(500);

            // Relationships - Updated to use ApplicationUser
            builder.HasOne(c => c.User)
                   .WithOne(u => u.Charity)
                   .HasForeignKey<Charity>(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Reservations)
                   .WithOne(r => r.Charity)
                   .HasForeignKey(r => r.CharityId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Needs)
                   .WithOne(n => n.Charity)
                   .HasForeignKey(n => n.CharityId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query Filter for Soft Delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
