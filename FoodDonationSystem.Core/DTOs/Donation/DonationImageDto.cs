namespace FoodDonationSystem.Core.DTOs.Donation
{
    public class DonationImageDto
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int DonationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

