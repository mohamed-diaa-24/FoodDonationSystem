using System.ComponentModel.DataAnnotations;

namespace FoodDonationSystem.Core.DTOs.Charity
{
    public class UpdateCapacityDto
    {
        [Required(ErrorMessage = "New capacity is required")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10000")]
        public int NewCapacity { get; set; }

        [StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
        public string? Reason { get; set; }
    }
}
