using FoodDonationSystem.Core.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodDonationSystem.Core.DTOs.Restaurant
{
    public class CreateRestaurantDto : CreateRestaurantRequest
    {
        public string? LicensePath { get; set; } = "";
        public string? RegisterPath { get; set; } = "";

    }

    public class CreateRestaurantRequest
    {
        [Required(ErrorMessage = "اسم المطعم مطلوب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "وصف المطعم مطلوب")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "خط العرض مطلوب")]
        [Range(-90, 90, ErrorMessage = "خط العرض غير صالح")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "خط الطول مطلوب")]
        [Range(-180, 180, ErrorMessage = "خط الطول غير صالح")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "ملف الترخيص مطلوب")]
        [AllowedExtensions([".pdf"])]
        public IFormFile LicenseDocument { get; set; } = default!;

        [Required(ErrorMessage = "السجل التجاري مطلوب")]
        [AllowedExtensions([".pdf"])]
        public IFormFile CommercialRegister { get; set; } = default!;
    }
}
