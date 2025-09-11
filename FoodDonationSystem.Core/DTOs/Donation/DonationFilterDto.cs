using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.DTOs.Donation
{
    public class DonationFilterDto
    {
        public DonationStatus? Status { get; set; }
        public string? FoodType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusKm { get; set; }
        public bool? RequiresPickup { get; set; }
        public bool? IsExpired { get; set; }
        public DateTime? ExpiryDateFrom { get; set; }
        public DateTime? ExpiryDateTo { get; set; }
        public int? MinServings { get; set; }
        public int? MaxServings { get; set; }
        public string? SearchTerm { get; set; }
    }
}

