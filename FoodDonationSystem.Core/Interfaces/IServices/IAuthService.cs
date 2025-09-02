using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Common;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token);
        Task<bool> LogoutAsync(string userId);
        string GenerateJwtToken(Guid userId, string email, List<string> roles);
    }
}
