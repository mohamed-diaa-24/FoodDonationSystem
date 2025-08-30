namespace FoodDonationSystem.Core.Entities
{
    public class VolunteerArea : BaseEntity
    {
        public string AreaName { get; set; } = string.Empty;
        public double CenterLatitude { get; set; }
        public double CenterLongitude { get; set; }
        public int RadiusKm { get; set; }

        public int VolunteerId { get; set; }
        public Volunteer Volunteer { get; set; } = null!;
    }
}
