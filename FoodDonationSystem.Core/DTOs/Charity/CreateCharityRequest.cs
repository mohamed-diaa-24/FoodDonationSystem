using FoodDonationSystem.Core.Attributes;
using FoodDonationSystem.Core.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodDonationSystem.Core.DTOs.Charity
{
    public class CreateCharityRequest
    {
        [Required(ErrorMessage = "Charity name is required")]
        [StringLength(100, ErrorMessage = "Charity name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10000")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Charity type is required")]
        public CharityType Type { get; set; }
        [Required(ErrorMessage = "ملف الترخيص مطلوب")]
        [AllowedExtensions([".pdf"])]
        public IFormFile? LicenseDocument { get; set; }

        [Required(ErrorMessage = "وثيقة الإثبات مطلوبة")]
        [AllowedExtensions([".pdf"])]
        public IFormFile? ProofDocument { get; set; }
    }

}
