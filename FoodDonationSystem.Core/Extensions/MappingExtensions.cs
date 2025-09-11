using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Charity;
using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Donation;
using FoodDonationSystem.Core.DTOs.Restaurant;
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Extensions
{
    public static class MappingExtensions
    {
        #region Mapping ApplicationUser
        public static UserInfoDto ToDto(this ApplicationUser user, List<string> roles)
        {
            return new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                ProfileImage = user.ProfileImage,
                Roles = roles,
                IsVerified = user.IsVerified,

            };
        }
        #endregion

        #region RestaurantMapping
        public static RestaurantDto ToDto(this Restaurant restaurant)
        {
            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Address = restaurant.Address,
                Latitude = restaurant.Latitude,
                Longitude = restaurant.Longitude,
                Status = restaurant.Status,
                IsActive = restaurant.IsActive,
                CreatedAt = restaurant.CreatedAt,
                OwnerName = restaurant.User != null ? $"{restaurant.User.FirstName} {restaurant.User.LastName}" : "",
                Email = restaurant.User?.Email ?? "",
                PhoneNumber = restaurant.User?.PhoneNumber ?? ""
            };
        }


        public static CreateRestaurantDto ToCreateRestaurantDto(this CreateRestaurantRequest request)
        {
            return new CreateRestaurantDto
            {
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                LicenseDocument = request.LicenseDocument,
                CommercialRegister = request.CommercialRegister,
            };
        }
        public static IEnumerable<RestaurantDto> ToDto(this IEnumerable<Restaurant> restaurants)
        {
            return restaurants.Select(r => r.ToDto());
        }

        public static Restaurant ToEntity(this CreateRestaurantDto dto, Guid userId)
        {
            return new Restaurant
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Status = ApprovalStatus.Pending,
                IsActive = true,
            };
        }

        public static Restaurant UpdateFromDto(this Restaurant restaurant, UpdateRestaurantDto dto)
        {
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.Address = dto.Address;
            restaurant.Latitude = dto.Latitude;
            restaurant.Longitude = dto.Longitude;
            restaurant.UpdatedAt = DateTime.UtcNow;
            return restaurant;
        }


        public static PagedResult<RestaurantDto> ToRestaurantPagedResult(
           this (IEnumerable<Restaurant> Items, int TotalCount) source,
           int pageNumber,
           int pageSize)
        {
            return source.ToPagedResult(pageNumber, pageSize, r => r.ToDto());
        }

        public static string ToDisplayName(this ApprovalStatus status)
        {
            return status.ToDisplayName<ApprovalStatus>();
        }
        #endregion


        #region Generic Page Mapping
        public static PagedResult<TDto> ToPagedResult<TEntity, TDto>(
          this IEnumerable<TEntity> items,
          int totalCount,
          int pageNumber,
          int pageSize,
          Func<TEntity, TDto> mapper)
        {
            return new PagedResult<TDto>
            {
                Items = items.Select(mapper),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public static PagedResult<TDto> ToPagedResult<TEntity, TDto>(
           this (IEnumerable<TEntity> Items, int TotalCount) source,
           int pageNumber,
           int pageSize,
           Func<TEntity, TDto> mapper)
        {
            return new PagedResult<TDto>
            {
                Items = source.Items.Select(mapper),
                TotalCount = source.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }



        public static PagedResult<TDto> ToManualPagedResult<TEntity, TDto>(
           this IEnumerable<TEntity> source,
           int pageNumber,
           int pageSize,
           Func<TEntity, TDto> mapper)
        {
            var totalCount = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(mapper);

            return new PagedResult<TDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        #endregion


        #region CharityMapping
        public static CharityDto ToDto(this Charity charity)
        {
            return new CharityDto
            {
                Id = charity.Id,
                Name = charity.Name,
                Description = charity.Description,
                Address = charity.Address,
                Latitude = charity.Latitude,
                Longitude = charity.Longitude,
                Capacity = charity.Capacity,
                Type = charity.Type,
                Status = charity.Status,
                IsActive = charity.IsActive,
                CreatedAt = charity.CreatedAt,
                ContactName = charity.User != null ? $"{charity.User.FirstName} {charity.User.LastName}" : "",
                Email = charity.User?.Email ?? "",
                PhoneNumber = charity.User?.PhoneNumber ?? ""
            };
        }

        public static IEnumerable<CharityDto> ToDto(this IEnumerable<Charity> charities)
        {
            return charities.Select(c => c.ToDto());
        }

        public static CreateCharityDto ToCreateCharityDto(this CreateCharityRequest request)
        {
            return new CreateCharityDto
            {
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                LicenseDocument = request.LicenseDocument,
                ProofDocument = request.ProofDocument,
                Capacity = request.Capacity,
            };
        }
        public static Charity ToEntity(this CreateCharityDto dto, Guid userId)
        {
            return new Charity
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Capacity = dto.Capacity,
                Type = dto.Type,
                Status = ApprovalStatus.Pending,
                IsActive = true,
            };
        }
        public static Charity UpdateFromDto(this Charity charity, UpdateCharityDto dto)
        {
            charity.Name = dto.Name;
            charity.Description = dto.Description;
            charity.Address = dto.Address;
            charity.Latitude = dto.Latitude;
            charity.Longitude = dto.Longitude;
            charity.Capacity = dto.Capacity;
            charity.Type = dto.Type;
            charity.UpdatedAt = DateTime.UtcNow;

            return charity;
        }

        public static PagedResult<CharityDto> ToCharityPagedResult(
           this (IEnumerable<Charity> Items, int TotalCount) source,
           int pageNumber,
           int pageSize)
        {
            return source.ToPagedResult(pageNumber, pageSize, c => c.ToDto());
        }
        #endregion

        #region DonationMapping
        public static DonationDto ToDto(this Donation donation)
        {
            return new DonationDto
            {
                Id = donation.Id,
                FoodType = donation.FoodType,
                Description = donation.Description,
                EstimatedServings = donation.EstimatedServings,
                ExpiryDateTime = donation.ExpiryDateTime,
                Status = donation.Status,
                RequiresPickup = donation.RequiresPickup,
                SpecialInstructions = donation.SpecialInstructions,
                ContactPerson = donation.ContactPerson,
                ContactPhone = donation.ContactPhone,
                CreatedAt = donation.CreatedAt,
                UpdatedAt = donation.UpdatedAt ?? donation.CreatedAt,
                RestaurantId = donation.RestaurantId,
                RestaurantName = donation.Restaurant?.Name ?? "",
                RestaurantAddress = donation.Restaurant?.Address ?? "",
                RestaurantPhone = donation.Restaurant?.User?.PhoneNumber ?? "",
                RestaurantLatitude = donation.Restaurant?.Latitude ?? 0,
                RestaurantLongitude = donation.Restaurant?.Longitude ?? 0,
                Images = donation.Images?.Select(i => i.ToDto()).ToList() ?? new List<DonationImageDto>(),
                ReservationCount = donation.Reservations?.Count ?? 0
            };
        }

        public static IEnumerable<DonationDto> ToDto(this IEnumerable<Donation> donations)
        {
            return donations.Select(d => d.ToDto());
        }

        public static Donation ToEntity(this CreateDonationDto dto, int restaurantId)
        {
            return new Donation
            {
                RestaurantId = restaurantId,
                FoodType = dto.FoodType,
                Description = dto.Description,
                EstimatedServings = dto.EstimatedServings,
                ExpiryDateTime = dto.ExpiryDateTime,
                Status = DonationStatus.Available,
                RequiresPickup = dto.RequiresPickup,
                SpecialInstructions = dto.SpecialInstructions,
                ContactPerson = dto.ContactPerson,
                ContactPhone = dto.ContactPhone
            };
        }

        public static Donation UpdateFromDto(this Donation donation, UpdateDonationDto dto)
        {
            donation.FoodType = dto.FoodType;
            donation.Description = dto.Description;
            donation.EstimatedServings = dto.EstimatedServings;
            donation.ExpiryDateTime = dto.ExpiryDateTime;
            donation.Status = dto.Status;
            donation.RequiresPickup = dto.RequiresPickup;
            donation.SpecialInstructions = dto.SpecialInstructions;
            donation.ContactPerson = dto.ContactPerson;
            donation.ContactPhone = dto.ContactPhone;
            donation.UpdatedAt = DateTime.UtcNow;
            return donation;
        }

        public static PagedResult<DonationDto> ToDonationPagedResult(
           this (IEnumerable<Donation> Items, int TotalCount) source,
           int pageNumber,
           int pageSize)
        {
            return source.ToPagedResult(pageNumber, pageSize, d => d.ToDto());
        }
        #endregion

        #region DonationImageMapping
        public static DonationImageDto ToDto(this DonationImage image)
        {
            return new DonationImageDto
            {
                Id = image.Id,
                ImagePath = image.ImagePath,
                IsPrimary = image.IsPrimary,
                DonationId = image.DonationId,
                CreatedAt = image.CreatedAt
            };
        }

        public static IEnumerable<DonationImageDto> ToDto(this IEnumerable<DonationImage> images)
        {
            return images.Select(i => i.ToDto());
        }

        public static DonationImage ToEntity(this CreateDonationImageDto dto)
        {
            return new DonationImage
            {
                DonationId = dto.DonationId,
                ImagePath = dto.ImagePath,
                IsPrimary = dto.IsPrimary
            };
        }
        #endregion

        public static string ToDisplayName<T>(this T enumValue) where T : Enum
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field != null)
            {
                var attribute = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                    .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }


            return enumValue.ToString();
        }


    }
}

