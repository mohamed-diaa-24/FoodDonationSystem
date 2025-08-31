namespace FoodDonationSystem.Core.Entities
{
    public class Volunteer : BaseEntity
    {
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
        public string? DriverLicense { get; set; }
        public bool IsActive { get; set; } = true;
        public int CompletedDeliveries { get; set; } = 0;

        // Navigation Properties
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public ICollection<VolunteerArea> ServiceAreas { get; set; } = new List<VolunteerArea>();
        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}
