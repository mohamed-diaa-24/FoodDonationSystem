using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Common;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<string>> ForgetPasswordAsync(ForgetPasswordRequestDto request);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
        Task<ApiResponse<string>> SendEmailConfirmationAsync(string email);
        Task<ApiResponse<string>> ConfirmEmailAsync(string email, string token);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token);
        Task<bool> LogoutAsync(string userId);
        string GenerateJwtToken(Guid userId, string email, List<string> roles);
    }
}
