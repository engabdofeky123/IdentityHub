using IdentityHub.DTOs.Auth;

namespace IdentityHub.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> LoginAsync(LoginDto loginDto);
        Task<AuthModel> RegisterAsync(RegisterDto registerDto);
        Task SignOut();
        Task<MeDto> GetMe(string userId);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeToken(string token);

    }
}
