using Microsoft.AspNetCore.Identity;

namespace FoodDonationSystem.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        // Navigation Properties
        public Restaurant? Restaurant { get; set; }
        public Charity? Charity { get; set; }
        public Volunteer? Volunteer { get; set; }
        public ICollection<Review> GivenReviews { get; set; } = new List<Review>();
        public ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
    }
}
