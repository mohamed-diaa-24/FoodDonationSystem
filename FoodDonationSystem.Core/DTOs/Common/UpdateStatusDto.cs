using FoodDonationSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FoodDonationSystem.Core.DTOs.Common
{
    public class UpdateStatusDto
    {
        [Required(ErrorMessage = "حالة الموافقة مطلوبة")]
        public ApprovalStatus Status { get; set; }

        public string? RejectionReason { get; set; }
    }
}
