namespace FoodDonationSystem.Core.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public UserInfoDto? User { get; set; }
    }
}
