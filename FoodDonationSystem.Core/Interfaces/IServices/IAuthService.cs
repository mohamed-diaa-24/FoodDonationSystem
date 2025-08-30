using FoodDonationSystem.Core.DTOs.Auth;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string token);
        Task<bool> LogoutAsync(string userId);
        string GenerateJwtToken(Guid userId, string email, List<string> roles);
    }
}
