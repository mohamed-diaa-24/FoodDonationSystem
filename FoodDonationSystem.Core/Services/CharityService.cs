using FoodDonationSystem.Core.DTOs.Charity;
using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Extensions;
using FoodDonationSystem.Core.Interfaces;
using FoodDonationSystem.Core.Interfaces.IServices;

namespace FoodDonationSystem.Core.Services
{
    public class CharityService : ICharityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CharityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<CharityDto>> RegisterCharityAsync(Guid userId, CreateCharityDto request)
        {
            try
            {
                // Check if user already has a charity
                var existingCharity = await _unitOfWork.Charities.GetByUserIdAsync(userId);
                if (existingCharity != null)
                {
                    return ApiResponse<CharityDto>.Failure("User already has a charity registered");
                }

                // Create new charity using extension method
                var charity = request.ToEntity(userId);

                await _unitOfWork.Charities.AddAsync(charity);
                await _unitOfWork.SaveChangesAsync();

                // Get the created charity with user info
                var createdCharity = await _unitOfWork.Charities.GetByUserIdAsync(userId);
                var charityDto = createdCharity!.ToDto();

                return ApiResponse<CharityDto>.Success(charityDto, "Charity registered successfully. Awaiting admin approval.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CharityDto>.Failure($"Error registering charity: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CharityDto>> GetCharityByUserIdAsync(Guid userId)
        {
            try
            {
                var charity = await _unitOfWork.Charities.GetByUserIdAsync(userId);
                if (charity == null)
                {
                    return ApiResponse<CharityDto>.Failure("Charity not found");
                }

                var charityDto = charity.ToDto();
                return ApiResponse<CharityDto>.Success(charityDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<CharityDto>.Failure($"Error retrieving charity: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CharityDto>> UpdateCharityAsync(Guid userId, UpdateCharityDto request)
        {
            try
            {
                var charity = await _unitOfWork.Charities.GetByUserIdAsync(userId);
                if (charity == null)
                {
                    return ApiResponse<CharityDto>.Failure("Charity not found");
                }

                // Update charity using extension method
                charity.UpdateFromDto(request);

                await _unitOfWork.Charities.UpdateAsync(charity);
                await _unitOfWork.SaveChangesAsync();

                var charityDto = charity.ToDto();
                return ApiResponse<CharityDto>.Success(charityDto, "Charity updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CharityDto>.Failure($"Error updating charity: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CharityDto>>> GetNearbyCharitiesAsync(double latitude, double longitude, double radiusKm)
        {
            try
            {
                var charities = await _unitOfWork.Charities.GetNearbyCharitiesAsync(latitude, longitude, radiusKm);
                var charityDtos = charities.ToDto();

                return ApiResponse<IEnumerable<CharityDto>>.Success(charityDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CharityDto>>.Failure($"Error retrieving nearby charities: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CharityDto>>> GetCharitiesByTypeAsync(CharityType type)
        {
            try
            {
                var charities = await _unitOfWork.Charities.GetCharitiesByTypeAsync(type);
                var charityDtos = charities.ToDto();

                return ApiResponse<IEnumerable<CharityDto>>.Success(charityDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CharityDto>>.Failure($"Error retrieving charities by type: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int charityId, ApprovalStatus status, string? rejectionReason = null)
        {
            try
            {
                var result = await _unitOfWork.Charities.UpdateStatusAsync(charityId, status, rejectionReason);
                if (!result)
                {
                    return ApiResponse<bool>.Failure("Charity not found");
                }

                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "Charity status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Error updating charity status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<CharityDto>>> GetCharitiesForAdminAsync(
            int pageNumber, int pageSize, ApprovalStatus? status = null, CharityType? type = null)
        {
            try
            {
                var charitiesResult = await _unitOfWork.Charities.GetCharitiesForAdminAsync(
                    pageNumber, pageSize, status, type);

                // Use extension method for mapping
                var result = charitiesResult.ToCharityPagedResult(pageNumber, pageSize);

                return ApiResponse<PagedResult<CharityDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<CharityDto>>.Failure($"Error retrieving charities: {ex.Message}");
            }
        }

    }
}
