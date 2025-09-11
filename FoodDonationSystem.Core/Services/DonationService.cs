using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Donation;
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Extensions;
using FoodDonationSystem.Core.Interfaces;
using FoodDonationSystem.Core.Interfaces.IServices;

namespace FoodDonationSystem.Core.Services
{
    public class DonationService : IDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public DonationService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<ApiResponse<DonationDto>> CreateDonationAsync(Guid userId, CreateDonationDto request)
        {
            try
            {

                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return ApiResponse<DonationDto>.Failure("لم يتم العثور على المطعم");
                }


                if (request.ExpiryDateTime <= DateTime.UtcNow)
                {
                    return ApiResponse<DonationDto>.Failure("تاريخ انتهاء الصلاحية يجب أن يكون في المستقبل");
                }


                var donation = request.ToEntity(restaurant.Id);
                await _unitOfWork.Donations.AddAsync(donation);
                await _unitOfWork.SaveChangesAsync();


                if (request.Images?.Any() == true)
                {
                    var donationImages = new List<DonationImage>();

                    foreach (var (image, index) in request.Images.Select((img, idx) => (img, idx)))
                    {

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            continue;
                        }


                        if (image.Length > 5 * 1024 * 1024)
                        {
                            continue;
                        }

                        // Upload file
                        var imagePath = await _fileService.SaveFileAsync(image, "donations");
                        if (string.IsNullOrEmpty(imagePath))
                        {
                            continue;
                        }

                        donationImages.Add(new DonationImage
                        {
                            DonationId = donation.Id,
                            ImagePath = imagePath,
                            IsPrimary = index == 0
                        });
                    }

                    if (donationImages.Any())
                    {
                        await _unitOfWork.DonationImages.AddRangeAsync(donationImages);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }


                var createdDonation = await _unitOfWork.Donations.GetByIdWithDetailsAsync(donation.Id);
                var donationDto = createdDonation!.ToDto();

                return ApiResponse<DonationDto>.Success(donationDto, "تم إنشاء التبرع بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<DonationDto>.Failure($"خطأ في إنشاء التبرع: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DonationDto>> GetDonationByIdAsync(int donationId)
        {
            try
            {
                var donation = await _unitOfWork.Donations.GetByIdWithDetailsAsync(donationId);
                if (donation == null)
                {
                    return ApiResponse<DonationDto>.Failure("لم يتم العثور على التبرع");
                }

                var donationDto = donation.ToDto();
                return ApiResponse<DonationDto>.Success(donationDto, "تم استرداد بيانات التبرع بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<DonationDto>.Failure($"خطأ في استرداد التبرع: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DonationDto>> UpdateDonationAsync(Guid userId, int donationId, UpdateDonationDto request)
        {
            try
            {

                var donation = await _unitOfWork.Donations.GetByIdWithDetailsAsync(donationId);
                if (donation == null)
                {
                    return ApiResponse<DonationDto>.Failure("لم يتم العثور على التبرع");
                }


                if (donation.Restaurant.UserId != userId)
                {
                    return ApiResponse<DonationDto>.Failure("ليس لديك صلاحية لتعديل هذا التبرع");
                }


                if (request.ExpiryDateTime <= DateTime.UtcNow)
                {
                    return ApiResponse<DonationDto>.Failure("تاريخ انتهاء الصلاحية يجب أن يكون في المستقبل");
                }


                donation.UpdateFromDto(request);
                await _unitOfWork.Donations.UpdateAsync(donation);
                await _unitOfWork.SaveChangesAsync();

                var donationDto = donation.ToDto();
                return ApiResponse<DonationDto>.Success(donationDto, "تم تحديث التبرع بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<DonationDto>.Failure($"خطأ في تحديث التبرع: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteDonationAsync(Guid userId, int donationId)
        {
            try
            {
                var donation = await _unitOfWork.Donations.GetByIdWithDetailsAsync(donationId);
                if (donation == null)
                {
                    return ApiResponse<bool>.Failure("لم يتم العثور على التبرع");
                }


                if (donation.Restaurant.UserId != userId)
                {
                    return ApiResponse<bool>.Failure("ليس لديك صلاحية لحذف هذا التبرع");
                }


                var hasActiveReservations = await _unitOfWork.Donations.HasActiveReservationsAsync(donationId);
                if (hasActiveReservations)
                {
                    return ApiResponse<bool>.Failure("لا يمكن حذف التبرع لأنه يحتوي على حجوزات نشطة");
                }


                await _unitOfWork.Donations.SoftDeleteAsync(donation);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "تم حذف التبرع بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"خطأ في حذف التبرع: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AdminDeleteDonationAsync(int donationId)
        {
            try
            {
                var donation = await _unitOfWork.Donations.GetByIdAsync(donationId);
                if (donation == null)
                {
                    return ApiResponse<bool>.Failure("لم يتم العثور على التبرع");
                }


                await _unitOfWork.Donations.SoftDeleteAsync(donation);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "تم حذف التبرع بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"خطأ في حذف التبرع: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<DonationDto>>> GetMyDonationsAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var donations = await _unitOfWork.Donations.GetByUserIdAsync(userId);
                var result = donations.ToManualPagedResult(pageNumber, pageSize, d => d.ToDto());

                return ApiResponse<PagedResult<DonationDto>>.Success(result, "تم استرداد تبرعاتك بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<DonationDto>>.Failure($"خطأ في استرداد تبرعاتك: {ex.Message}");
            }
        }


        public async Task<ApiResponse<PagedResult<DonationDto>>> GetAvailableDonationsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var donations = await _unitOfWork.Donations.GetAvailableDonationsAsync();
                var result = donations.ToManualPagedResult(pageNumber, pageSize, d => d.ToDto());

                return ApiResponse<PagedResult<DonationDto>>.Success(result, "تم استرداد التبرعات المتاحة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<DonationDto>>.Failure($"خطأ في استرداد التبرعات المتاحة: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<DonationDto>>> GetNearbyDonationsAsync(double latitude, double longitude, double radiusKm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var donations = await _unitOfWork.Donations.GetNearbyDonationsAsync(latitude, longitude, radiusKm);
                var result = donations.ToManualPagedResult(pageNumber, pageSize, d => d.ToDto());

                return ApiResponse<PagedResult<DonationDto>>.Success(result, "تم استرداد التبرعات القريبة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<DonationDto>>.Failure($"خطأ في استرداد التبرعات القريبة: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<DonationDto>>> GetNearbyDonationsForCharityAsync(Guid charityUserId, double radiusKm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var charity = await _unitOfWork.Charities.GetByUserIdAsync(charityUserId);
                if (charity == null)
                {
                    return ApiResponse<PagedResult<DonationDto>>.Failure("لم يتم العثور على الجمعية الخيرية");
                }

                var donations = await _unitOfWork.Donations.GetNearbyDonationsAsync(charity.Latitude, charity.Longitude, radiusKm);
                var result = donations.ToManualPagedResult(pageNumber, pageSize, d => d.ToDto());

                return ApiResponse<PagedResult<DonationDto>>.Success(result, "تم استرداد التبرعات القريبة من الجمعية الخيرية بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<DonationDto>>.Failure($"خطأ في استرداد التبرعات القريبة: {ex.Message}");
            }
        }



        public async Task<ApiResponse<bool>> AdminUpdateDonationStatusAsync(int donationId, DonationStatus status)
        {
            try
            {
                var success = await _unitOfWork.Donations.UpdateStatusAsync(donationId, status);
                if (!success)
                {
                    return ApiResponse<bool>.Failure("لم يتم العثور على التبرع");
                }

                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "تم تحديث حالة التبرع بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"خطأ في تحديث حالة التبرع: {ex.Message}");
            }
        }


        public async Task<ApiResponse<DonationImageDto>> AddDonationImageAsync(Guid userId, int donationId, CreateDonationImageDto request)
        {
            try
            {
                var donation = await _unitOfWork.Donations.GetByIdWithDetailsAsync(donationId);
                if (donation == null)
                {
                    return ApiResponse<DonationImageDto>.Failure("لم يتم العثور على التبرع");
                }

                if (donation.Restaurant.UserId != userId)
                {
                    return ApiResponse<DonationImageDto>.Failure("ليس لديك صلاحية لإضافة صورة لهذا التبرع");
                }


                if (request.IsPrimary)
                {
                    var existingImages = await _unitOfWork.DonationImages.FindAsync(i => i.DonationId == donationId && i.IsPrimary);
                    foreach (var image in existingImages)
                    {
                        image.IsPrimary = false;
                        await _unitOfWork.DonationImages.UpdateAsync(image);
                    }
                }

                var donationImage = request.ToEntity();
                await _unitOfWork.DonationImages.AddAsync(donationImage);
                await _unitOfWork.SaveChangesAsync();

                var imageDto = donationImage.ToDto();
                return ApiResponse<DonationImageDto>.Success(imageDto, "تم إضافة الصورة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<DonationImageDto>.Failure($"خطأ في إضافة الصورة: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RemoveDonationImageAsync(Guid userId, int donationId, int imageId)
        {
            try
            {
                var donation = await _unitOfWork.Donations.GetByIdWithDetailsAsync(donationId);
                if (donation == null)
                {
                    return ApiResponse<bool>.Failure("لم يتم العثور على التبرع");
                }


                if (donation.Restaurant.UserId != userId)
                {
                    return ApiResponse<bool>.Failure("ليس لديك صلاحية لحذف صورة من هذا التبرع");
                }

                var image = await _unitOfWork.DonationImages.GetByIdAsync(imageId);
                if (image == null || image.DonationId != donationId)
                {
                    return ApiResponse<bool>.Failure("لم يتم العثور على الصورة");
                }

                await _unitOfWork.DonationImages.SoftDeleteAsync(image);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "تم حذف الصورة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"خطأ في حذف الصورة: {ex.Message}");
            }
        }



        public async Task<ApiResponse<PagedResult<DonationDto>>> GetDonationsForAdminAsync(int pageNumber = 1, int pageSize = 10, DonationStatus? status = null, string? searchTerm = null)
        {
            try
            {
                var result = await _unitOfWork.Donations.GetDonationsForAdminAsync(pageNumber, pageSize, status, searchTerm);
                var pagedResult = result.ToDonationPagedResult(pageNumber, pageSize);

                return ApiResponse<PagedResult<DonationDto>>.Success(pagedResult, "تم استرداد قائمة التبرعات بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<DonationDto>>.Failure($"خطأ في استرداد قائمة التبرعات: {ex.Message}");
            }
        }

    }
}
