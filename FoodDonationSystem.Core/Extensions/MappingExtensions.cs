using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.Entities;

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
    }
}
