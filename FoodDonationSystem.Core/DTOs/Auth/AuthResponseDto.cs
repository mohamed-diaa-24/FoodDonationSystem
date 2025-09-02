namespace FoodDonationSystem.Core.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public UserInfoDto? User { get; set; }
    }
}
