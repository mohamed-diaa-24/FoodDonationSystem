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
                    return ApiResponse<CharityDto>.Failure("المستخدم لديه جمعية خيرية مسجلة بالفعل");
                }

                // Create new charity using extension method
                var charity = request.ToEntity(userId);

                await _unitOfWork.Charities.AddAsync(charity);
                await _unitOfWork.SaveChangesAsync();

                // Get the created charity with user info
                var createdCharity = await _unitOfWork.Charities.GetByUserIdAsync(userId);
                var charityDto = createdCharity!.ToDto();

                return ApiResponse<CharityDto>.Success(charityDto, "تم تسجيل الجمعية الخيرية بنجاح. بانتظار موافقة الإدارة.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CharityDto>.Failure($"خطأ في تسجيل الجمعية الخيرية: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CharityDto>> GetCharityByUserIdAsync(Guid userId)
        {
            try
            {
                var charity = await _unitOfWork.Charities.GetByUserIdAsync(userId);
                if (charity == null)
                {
                    return ApiResponse<CharityDto>.Failure("لم يتم العثور على الجمعية الخيرية");
                }

                var charityDto = charity.ToDto();
                return ApiResponse<CharityDto>.Success(charityDto, "تم استرداد بيانات الجمعية الخيرية بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<CharityDto>.Failure($"خطأ في استرداد الجمعية الخيرية: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CharityDto>> UpdateCharityAsync(Guid userId, UpdateCharityDto request)
        {
            try
            {
                var charity = await _unitOfWork.Charities.GetByUserIdAsync(userId);
                if (charity == null)
                {
                    return ApiResponse<CharityDto>.Failure("لم يتم العثور على الجمعية الخيرية");
                }

                // Update charity using extension method
                charity.UpdateFromDto(request);

                await _unitOfWork.Charities.UpdateAsync(charity);
                await _unitOfWork.SaveChangesAsync();

                var charityDto = charity.ToDto();
                return ApiResponse<CharityDto>.Success(charityDto, "تم تحديث الجمعية الخيرية بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<CharityDto>.Failure($"حدث خطأ أثناء تحديث الجمعية الخيرية: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<CharityDto>>> GetNearbyCharitiesAsync(
           double latitude, double longitude, double radiusKm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var charities = await _unitOfWork.Charities.GetNearbyCharitiesAsync(latitude, longitude, radiusKm);

                // Use extension method for manual pagination
                var result = charities.ToManualPagedResult(pageNumber, pageSize, c => c.ToDto());

                return ApiResponse<PagedResult<CharityDto>>.Success(result, "تم استرداد الجمعيات الخيرية القريبة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<CharityDto>>.Failure($"حدث خطأ أثناء استرداد الجمعيات الخيرية القريبة: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<CharityDto>>> GetNearbyCharitiesForRestaurantAsync(
           Guid restaurantUserId, double radiusKm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(restaurantUserId);
                if (restaurant == null)
                {
                    return ApiResponse<PagedResult<CharityDto>>.Failure("لم يتم العثور على المطعم");
                }

                var charities = await _unitOfWork.Charities.GetNearbyCharitiesAsync(
                    restaurant.Latitude, restaurant.Longitude, radiusKm);

                var result = charities.ToManualPagedResult(pageNumber, pageSize, c => c.ToDto());

                return ApiResponse<PagedResult<CharityDto>>.Success(result, "تم استرداد الجمعيات الخيرية القريبة من المطعم بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<CharityDto>>.Failure($"حدث خطأ أثناء استرداد الجمعيات الخيرية القريبة: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CharityDto>>> GetCharitiesByTypeAsync(CharityType type)
        {
            try
            {
                var charities = await _unitOfWork.Charities.GetCharitiesByTypeAsync(type);
                var charityDtos = charities.ToDto();

                return ApiResponse<IEnumerable<CharityDto>>.Success(charityDtos, "تم استرداد الجمعيات الخيرية حسب النوع بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CharityDto>>.Failure($"حدث خطأ أثناء استرداد الجمعيات الخيرية حسب النوع: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int charityId, ApprovalStatus status, string? rejectionReason = null)
        {
            try
            {
                var result = await _unitOfWork.Charities.UpdateStatusAsync(charityId, status, rejectionReason);
                if (!result)
                {
                    return ApiResponse<bool>.Failure("لم يتم العثور على الجمعية الخيرية");
                }

                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "تم تحديث حالة الجمعية الخيرية بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"حدث خطأ أثناء تحديث حالة الجمعية الخيرية: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<CharityDto>>> GetCharitiesForAdminAsync(
            int pageNumber, int pageSize, ApprovalStatus? status = null, string? searchTerm = null)
        {
            try
            {
                var charitiesResult = await _unitOfWork.Charities.GetCharitiesForAdminAsync(
                    pageNumber, pageSize, status, searchTerm);

                // Use extension method for mapping
                var result = charitiesResult.ToCharityPagedResult(pageNumber, pageSize);

                return ApiResponse<PagedResult<CharityDto>>.Success(result, "تم استرداد قائمة الجمعيات الخيرية بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<CharityDto>>.Failure($"خطأ في استرداد الجمعيات الخيرية: {ex.Message}");
            }
        }

    }
}
