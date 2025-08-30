using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Entities
{
    public class Charity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Capacity { get; set; }
        public string? LicenseDocument { get; set; }
        public string? ProofDocument { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public string? RejectionReason { get; set; }
        public bool IsActive { get; set; } = true;
        public double AverageRating { get; set; } = 0;
        public int TotalReceived { get; set; } = 0;
        public CharityType Type { get; set; }

        // Navigation Properties
        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<CharityNeed> Needs { get; set; } = new List<CharityNeed>();
    }
}
