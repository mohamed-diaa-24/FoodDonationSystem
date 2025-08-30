using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Entities
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public ReviewType Type { get; set; }

        // Navigation Properties
        public Guid FromUserId { get; set; }

        public ApplicationUser FromUser { get; set; } = null!;
        public Guid ToUserId { get; set; }
        public ApplicationUser ToUser { get; set; } = null!;
        public int? ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
    }
}
