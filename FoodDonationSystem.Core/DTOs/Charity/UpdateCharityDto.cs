using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.DTOs.Charity
{
    public class UpdateCharityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Capacity { get; set; }
        public CharityType Type { get; set; }
    }
}
