namespace FoodDonationSystem.Core.Entities
{
    public class CharityNeed : BaseEntity
    {
        public string FoodType { get; set; } = string.Empty;
        public int RequiredServings { get; set; }
        public string? Description { get; set; }
        public DateTime ValidUntil { get; set; }
        public int CharityId { get; set; }

        public Charity Charity { get; set; } = null!;
    }
}
