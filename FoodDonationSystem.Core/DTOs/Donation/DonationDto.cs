using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.DTOs.Donation
{
    public class DonationDto
    {
        public int Id { get; set; }
        public string FoodType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedServings { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public DonationStatus Status { get; set; }
        public bool RequiresPickup { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Restaurant Information
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string RestaurantAddress { get; set; } = string.Empty;
        public string RestaurantPhone { get; set; } = string.Empty;
        public double RestaurantLatitude { get; set; }
        public double RestaurantLongitude { get; set; }

        // Images
        public List<DonationImageDto> Images { get; set; } = new List<DonationImageDto>();

        // Statistics
        public int ReservationCount { get; set; }
        public bool IsExpired => ExpiryDateTime < DateTime.UtcNow;
        public bool IsAvailable => Status == DonationStatus.Available && !IsExpired;
    }
}

