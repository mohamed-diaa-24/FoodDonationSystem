using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Extensions;

namespace FoodDonationSystem.Core.DTOs.Restaurant
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ApprovalStatus Status { get; set; }
        public string StatusDisplayName => Status.ToDisplayName();

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // User info
        public string OwnerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
