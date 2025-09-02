using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Common;
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
                LicenseDocument = dto.LicenseDocument,
                CommercialRegister = dto.CommercialRegister,
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

