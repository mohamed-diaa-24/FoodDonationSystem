using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.DTOs.Donation
{
    public class UpdateDonationDto
    {
        public string FoodType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedServings { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public DonationStatus Status { get; set; }
        public bool RequiresPickup { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
    }
}

