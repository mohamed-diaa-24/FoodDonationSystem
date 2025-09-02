using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Extensions;
using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {

                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {

                    return ApiResponse<AuthResponseDto>.Failure("User already exists with this email address");
                }

                if (request.Role.ToLower() == "Admin".ToLower() || !await _roleManager.RoleExistsAsync(request.Role))
                {

                    return ApiResponse<AuthResponseDto>.Failure("The  user Role is invalid.");
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
                return ApiResponse<AuthResponseDto>.Success(data);

            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.Failure($"An error occurred while creating the account.: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponse<AuthResponseDto>.Failure("Incorrect email or password");
                }


                if (!user.IsActive)
                {
                    return ApiResponse<AuthResponseDto>.Failure("The account is disabled, please contact the administration");
                }


                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {

                    return ApiResponse<AuthResponseDto>.Failure("Incorrect email or password");
                }


                var roles = await _userManager.GetRolesAsync(user);


                var token = GenerateJwtToken(user.Id, user.Email, roles.ToList());

                var data = new AuthResponseDto
                {

                    Token = token,
                    TokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:ExpiryDays"])),
                    User = user.ToDto(roles.ToList())
                };
                return ApiResponse<AuthResponseDto>.Success(data);
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.Failure($"An error occurred while logging in.: {ex.Message}");
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

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string token)
        {
            // TODO: Implement refresh token logic
            throw new NotImplementedException();
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            // TODO: Implement logout logic (blacklist token if needed)
            await Task.CompletedTask;
            return true;
        }
    }
}
