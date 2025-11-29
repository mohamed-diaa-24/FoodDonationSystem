namespace FoodDonationSystem.Core.Entities
{
    public class CharityImage : BaseEntity
    {
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        public int CharityId { get; set; }
        public Charity Charity { get; set; } = null!;
    }
}

