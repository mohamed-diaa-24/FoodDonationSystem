using Microsoft.AspNetCore.Http;

namespace FoodDonationSystem.Core.DTOs.Donation
{
    public class CreateDonationDto
    {
        public string FoodType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedServings { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public bool RequiresPickup { get; set; } = true;
        public string? SpecialInstructions { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
