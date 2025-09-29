using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Extensions;

namespace FoodDonationSystem.Core.DTOs.Reservation
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime ReservationTime { get; set; }
        public ReservationStatus Status { get; set; }
        public string StatusDisplayName => Status.ToDisplayName();
        public string? Notes { get; set; }
        public DateTime? PickupTime { get; set; }
        public string? PickupPersonName { get; set; }
        public string? PickupPersonPhone { get; set; }

        // Links
        public int DonationId { get; set; }
        public int CharityId { get; set; }

        // Projections for convenience
        public string DonationFoodType { get; set; } = string.Empty;
        public DateTime DonationExpiry { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string RestaurantPhone { get; set; } = string.Empty;
        public string RestaurantAddress { get; set; } = string.Empty;
    }
}
