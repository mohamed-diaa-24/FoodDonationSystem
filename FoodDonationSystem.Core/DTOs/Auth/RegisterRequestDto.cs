using System.ComponentModel.DataAnnotations;

namespace FoodDonationSystem.Core.DTOs.Auth
{
    public class RegisterRequestDto
    {

        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم الأول لا يجب أن يزيد عن 50 حرف")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        [StringLength(50, ErrorMessage = "لا يجب أن يتجاوز الاسم الأخير 50 ​​حرفًا")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب أن تكون بين 6 و 100 حرف")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "مطلوب تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور والتأكيد غير متطابقين")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "دور المستخدم مطلوب")]
        public string Role { get; set; } = string.Empty; // "Restaurant", "Charity", "Volunteer", "Individual"
    }
}
