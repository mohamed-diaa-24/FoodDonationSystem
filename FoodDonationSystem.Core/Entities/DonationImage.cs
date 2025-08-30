namespace FoodDonationSystem.Core.Entities
{
    public class DonationImage : BaseEntity
    {
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        public int DonationId { get; set; }
        public Donation Donation { get; set; } = null!;
    }
}
