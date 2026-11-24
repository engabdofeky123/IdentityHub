using IdentityHub.Data;
using IdentityHub.DTOs.Auth;
using IdentityHub.Helpers;
using IdentityHub.Interfaces;
using IdentityHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityHub.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> options, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _jwtOptions = options.Value;
            _signInManager = signInManager;
        }


        public async Task<AuthModel> LoginAsync(LoginDto loginDto)
        {
            ApplicationUser? user;

            if (loginDto.EmailOrPhone.Contains('@'))
            {
                // Login using Email
                user = await _userManager.FindByEmailAsync(loginDto.EmailOrPhone);
            }
            else
            {
                // Login using Phone
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == loginDto.EmailOrPhone);
            }

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new AuthModel
                {
                    Message = "Email/Phone or Password is not valid"
                };
            }

            var jwtToken = await GenerateToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            return new AuthModel
            {
                ExpiresON = jwtToken.ValidTo,
                EmailOrPhone = loginDto.EmailOrPhone,
                IsAuthenticated = true,
                Roles = rolesList.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Message = "Welcome again ^_^"
            };

        }

        public async Task<AuthModel> RegisterAsync(RegisterDto dto)
        {
            var FoundMail = await _userManager.FindByEmailAsync(dto.Email);
            var FoundByUserName = await _userManager.FindByNameAsync(dto.FirstName + ' ' + dto.LastName);

            if (FoundByUserName != null || FoundMail != null)
                return new AuthModel { Message = "Email or UserName are already exist" };

            var newAppUser = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.FirstName + ' ' + dto.LastName,
            };

            var result = await _userManager.CreateAsync(newAppUser, dto.Password);

            if (!result.Succeeded)
            {
                var ErrorList = "";
                foreach (var error in result.Errors)
                    ErrorList += error.Description + ' ';
                return new AuthModel { Message = $"There are some errors : {ErrorList}" };
            }
            await _userManager.AddToRoleAsync(newAppUser, "user");

            // Generate Token

            var token = await GenerateToken(newAppUser);
            return new AuthModel
            {
                EmailOrPhone = newAppUser.Email,
                ExpiresON = token.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "user" },
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = newAppUser.UserName
            };

        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Invalid Token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Inactive Token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = await GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Failed to update token";
                return authModel;
            }

            var jwt = await GenerateToken(user);
            authModel.IsAuthenticated = true;
            authModel.Message = "Successfully";
            authModel.EmailOrPhone = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeToken(string token)
        {
            var authModel = new AuthModel();
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = await GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            var result = await _userManager.UpdateAsync(user);

            return true;
        }

        private async Task<RefreshToken> GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var generator = new RNGCryptoServiceProvider();

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        public async Task<MeDto> GetMe(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new InvalidOperationException("User not found");

            return new MeDto
            {
                Name = user.UserName ?? "N/A",
                Email = user.Email ?? "N/A",
                isEmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber ?? "N/A"
            };
        }

        // Generate Tokens
        private async Task<SecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles
                .Select(role => new Claim(ClaimTypes.Role, role))
                .ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("userID" , user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.Key)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtOptions.ExoiresOn30Days),
                signingCredentials: creds
            );

            return token;
        }
    }
}
