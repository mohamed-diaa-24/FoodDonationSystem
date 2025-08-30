namespace FoodDonationSystem.Core.Entities
{
    public class RestaurantSchedule : BaseEntity
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan DonationTime { get; set; }
        public bool IsActive { get; set; } = true;

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;
    }
}
