using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Entities
{
    public class Donation : BaseEntity
    {
        public string FoodType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedServings { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public DonationStatus Status { get; set; } = DonationStatus.Available;
        public bool RequiresPickup { get; set; } = true;
        public string? SpecialInstructions { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }

        // Navigation Properties
        public int RestaurantId { get; set; }

        public Restaurant Restaurant { get; set; } = null!;
        public ICollection<DonationImage> Images { get; set; } = new List<DonationImage>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
