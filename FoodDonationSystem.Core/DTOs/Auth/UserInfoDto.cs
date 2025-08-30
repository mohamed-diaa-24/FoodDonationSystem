namespace FoodDonationSystem.Core.DTOs.Auth
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
    }
}
