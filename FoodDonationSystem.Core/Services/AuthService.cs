using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Extensions;
using FoodDonationSystem.Core.Interfaces;
using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace FoodDonationSystem.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly string _baseUrl = string.Empty;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _baseUrl = configuration["AppSettings:BaseUrl"] ?? string.Empty;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {

                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {

                    return ApiResponse<AuthResponseDto>.Failure("المستخدم موجود بالفعل بهذا البريد الإلكتروني");
                }

                if (request.Role.ToLower() == "Admin".ToLower() || !await _roleManager.RoleExistsAsync(request.Role))
                {

                    return ApiResponse<AuthResponseDto>.Failure("دور المستخدم غير صالح أو غير مسموح");
                }


                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    IsActive = true,
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow
                };


                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {

                    var message = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ApiResponse<AuthResponseDto>.Failure(message);
                }


                await _userManager.AddToRoleAsync(user, request.Role);


                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user.Id, user.Email, roles.ToList());

                var data = new AuthResponseDto
                {
                    Token = token,
                    TokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:ExpiryDays"])),
                    User = user.ToDto(roles.ToList())
                };
                return ApiResponse<AuthResponseDto>.Success(data, "تم إنشاء الحساب بنجاح");

            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.Failure($"حدث خطأ أثناء إنشاء الحساب: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponse<AuthResponseDto>.Failure("البريد الإلكتروني أو كلمة المرور غير صحيحة");
                }


                if (!user.IsActive)
                {
                    return ApiResponse<AuthResponseDto>.Failure("تم تعطيل الحساب الرجاء التواصل مع الإدارة");
                }


                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {

                    return ApiResponse<AuthResponseDto>.Failure("البريد الإلكتروني أو كلمة المرور غير صحيحة");
                }


                var roles = await _userManager.GetRolesAsync(user);


                var token = GenerateJwtToken(user.Id, user.Email, roles.ToList());

                var data = new AuthResponseDto
                {

                    Token = token,
                    TokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:ExpiryDays"])),
                    User = user.ToDto(roles.ToList())
                };
                return ApiResponse<AuthResponseDto>.Success(data, "تم تسجيل الدخول بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.Failure($"حدث خطأ أثناء تسجيل الدخول: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> ForgetPasswordAsync(ForgetPasswordRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponse<string>.Success("إذا كان البريد الإلكتروني موجود، سيتم إرسال رابط إعادة تعيين كلمة المرور");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);


                var encodedToken = WebUtility.UrlEncode(token);
                var resetUrl = $"{_baseUrl}/api/open-app/reset-password?email={request.Email}&token={encodedToken}";


                await _emailService.SendPasswordResetEmailAsync(user.Email, resetUrl, user.FirstName);

                return ApiResponse<string>.Success("إذا كان البريد الإلكتروني موجود، سيتم إرسال رابط إعادة تعيين كلمة المرور", "تم إرسال رابط إعادة تعيين كلمة المرور");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure($"حدث خطأ أثناء إرسال رابط إعادة تعيين كلمة المرور: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponse<string>.Failure("المستخدم غير موجود");
                }

                // فك ترميز الرمز
                var decodedToken = WebUtility.UrlDecode(request.Token);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ApiResponse<string>.Failure($"فشل في إعادة تعيين كلمة المرور: {errors}");
                }

                return ApiResponse<string>.Success("تم إعادة تعيين كلمة المرور بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure($"حدث خطأ أثناء إعادة تعيين كلمة المرور: {ex.Message}");
            }
        }
        public string GenerateJwtToken(Guid userId, string email, List<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };


            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(jwtSettings["ExpiryDays"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return ApiResponse<string>.Failure("المستخدم غير موجود");
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ApiResponse<string>.Failure($"فشل في تغيير كلمة المرور: {errors}");
                }

                return ApiResponse<string>.Success("تم تغيير كلمة المرور بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure($"حدث خطأ أثناء تغيير كلمة المرور: {ex.Message}");
            }
        }
        public async Task<ApiResponse<string>> SendEmailConfirmationAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return ApiResponse<string>.Failure("المستخدم غير موجود");
                }

                if (user.EmailConfirmed)
                {
                    return ApiResponse<string>.Success("البريد الإلكتروني مؤكد بالفعل");
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var confirmationUrl = $"{_baseUrl}/api/open-app/confirm-email?email={email}&token={encodedToken}";

                await _emailService.SendEmailConfirmationAsync(user.Email, confirmationUrl, user.FirstName);

                return ApiResponse<string>.Success("تم إرسال رابط تأكيد البريد الإلكتروني");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure($"حدث خطأ أثناء إرسال رابط تأكيد البريد الإلكتروني: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return ApiResponse<string>.Failure("المستخدم غير موجود");
                }

                if (user.EmailConfirmed)
                {
                    return ApiResponse<string>.Success("البريد الإلكتروني مؤكد بالفعل");
                }

                var decodedToken = WebUtility.UrlDecode(token);
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ApiResponse<string>.Failure($"فشل في تأكيد البريد الإلكتروني: {errors}");
                }

                // تحديث حالة التحقق
                user.IsVerified = true;
                await _userManager.UpdateAsync(user);

                return ApiResponse<string>.Success("تم تأكيد البريد الإلكتروني بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure($"حدث خطأ أثناء تأكيد البريد الإلكتروني: {ex.Message}");
            }
        }
        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

                // التحقق من الرمز المميز
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return ApiResponse<AuthResponseDto>.Failure("رمز غير صالح");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return ApiResponse<AuthResponseDto>.Failure("المستخدم غير موجود أو غير نشط");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var newToken = GenerateJwtToken(user.Id, user.Email, roles.ToList());

                var data = new AuthResponseDto
                {
                    Token = newToken,
                    TokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:ExpiryDays"])),
                    User = user.ToDto(roles.ToList())
                };

                return ApiResponse<AuthResponseDto>.Success(data, "تم تحديث الرمز المميز بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.Failure($"فشل في تحديث الرمز المميز: {ex.Message}");
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            // TODO: Implement logout logic (blacklist token if needed)
            await Task.CompletedTask;
            return true;
        }

        public async Task<ApiResponse<bool>> DeleteMyAccountAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return ApiResponse<bool>.Failure("المستخدم غير موجود");
                }

                // Soft delete: mark as deleted/inactive
                user.IsActive = false;
                user.IsDeleted = true;
                await _userManager.UpdateAsync(user);

                // Soft delete navigation roots (Restaurant/Charity) if exist
                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(user.Id);
                if (restaurant != null)
                {
                    await _unitOfWork.Restaurants.SoftDeleteAsync(restaurant);
                }

                var charity = await _unitOfWork.Charities.GetByUserIdAsync(user.Id);
                if (charity != null)
                {
                    await _unitOfWork.Charities.SoftDeleteAsync(charity);
                }

                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "تم حذف الحساب بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"حدث خطأ أثناء حذف الحساب: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AdminDeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return ApiResponse<bool>.Failure("المستخدم غير موجود");
                }

                user.IsActive = false;
                user.IsDeleted = true;
                await _userManager.UpdateAsync(user);

                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(user.Id);
                if (restaurant != null)
                {
                    await _unitOfWork.Restaurants.SoftDeleteAsync(restaurant);
                }

                var charity = await _unitOfWork.Charities.GetByUserIdAsync(user.Id);
                if (charity != null)
                {
                    await _unitOfWork.Charities.SoftDeleteAsync(charity);
                }

                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "تم حذف المستخدم بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"حدث خطأ أثناء حذف المستخدم: {ex.Message}");
            }
        }
    }
}
