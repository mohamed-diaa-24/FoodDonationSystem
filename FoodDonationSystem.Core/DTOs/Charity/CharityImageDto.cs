namespace FoodDonationSystem.Core.DTOs.Charity
{
    public class CharityImageDto
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int CharityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

