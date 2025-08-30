using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Entities
{
    public class Delivery : BaseEntity
    {
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Assigned;
        public string? DeliveryNotes { get; set; }
        public string? ProofOfDelivery { get; set; }

        // Navigation Properties
        public int ReservationId { get; set; }

        public Reservation Reservation { get; set; } = null!;
        public int? VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }

    }
}
