using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Entities
{
    public class Reservation : BaseEntity
    {
        public DateTime ReservationTime { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public string? Notes { get; set; }
        public DateTime? PickupTime { get; set; }
        public string? PickupPersonName { get; set; }
        public string? PickupPersonPhone { get; set; }

        // Navigation Properties
        public int DonationId { get; set; }

        public Donation Donation { get; set; } = null!;
        public int CharityId { get; set; }

        public Charity Charity { get; set; } = null!;
    }
}
