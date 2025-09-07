using FoodDonationSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FoodDonationSystem.Core.DTOs.Charity
{
    public class UpdateCharityDto
    {
        [Required(ErrorMessage = "اسم الجمعية الخيرية مطلوب")]
        [StringLength(100, ErrorMessage = "اسم الجمعية الخيرية لا يجب أن يزيد عن 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "الوصف لا يجب أن يزيد عن 500 حرف")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(200, ErrorMessage = "العنوان لا يجب أن يزيد عن 200 حرف")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "خط العرض مطلوب")]
        [Range(-90, 90, ErrorMessage = "خط العرض يجب أن يكون بين -90 و 90")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "خط الطول مطلوب")]
        [Range(-180, 180, ErrorMessage = "خط الطول يجب أن يكون بين -180 و 180")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "السعة مطلوبة")]
        [Range(1, 10000, ErrorMessage = "السعة يجب أن تكون بين 1 و 10000")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "نوع الجمعية الخيرية مطلوب")]
        public CharityType Type { get; set; }
    }
}
