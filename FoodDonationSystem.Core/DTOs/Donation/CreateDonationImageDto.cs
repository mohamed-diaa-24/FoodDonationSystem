namespace FoodDonationSystem.Core.DTOs.Donation
{
    public class CreateDonationImageDto
    {
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
        public int DonationId { get; set; }
    }
}

