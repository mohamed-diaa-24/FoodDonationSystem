using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Donation;
using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IDonationService
    {
        // Basic CRUD Operations
        Task<ApiResponse<DonationDto>> CreateDonationAsync(Guid userId, CreateDonationDto request);
        Task<ApiResponse<DonationDto>> GetDonationByIdAsync(int donationId);
        Task<ApiResponse<DonationDto>> UpdateDonationAsync(Guid userId, int donationId, UpdateDonationDto request);
        Task<ApiResponse<bool>> DeleteDonationAsync(Guid userId, int donationId);
        Task<ApiResponse<bool>> AdminDeleteDonationAsync(int donationId);

        // Restaurant Donation Management
        Task<ApiResponse<PagedResult<DonationDto>>> GetMyDonationsAsync(Guid userId, int pageNumber = 1, int pageSize = 10);

        // Public Donation Browsing
        Task<ApiResponse<PagedResult<DonationDto>>> GetAvailableDonationsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<DonationDto>>> GetNearbyDonationsAsync(double latitude, double longitude, double radiusKm, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<DonationDto>>> GetNearbyDonationsForCharityAsync(Guid charityUserId, double radiusKm, int pageNumber = 1, int pageSize = 10);
        // Image Management
        Task<ApiResponse<DonationImageDto>> AddDonationImageAsync(Guid userId, int donationId, CreateDonationImageDto request);
        Task<ApiResponse<bool>> RemoveDonationImageAsync(Guid userId, int donationId, int imageId);

        // Admin Operations
        Task<ApiResponse<PagedResult<DonationDto>>> GetDonationsForAdminAsync(int pageNumber = 1, int pageSize = 10, DonationStatus? status = null, string? searchTerm = null);
        Task<ApiResponse<bool>> AdminUpdateDonationStatusAsync(int donationId, DonationStatus status);
    }
}
