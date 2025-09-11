using FoodDonationSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FoodDonationSystem.Core.DTOs.Donation
{
    public class UpdateDonationStatusDto
    {
        [Required(ErrorMessage = "حالة التبرع مطلوبة")]
        public DonationStatus Status { get; set; }

        public string? RejectionReason { get; set; }
    }
}

