using Ecommerce.Shared.DTOs;

namespace Ecommerce.WebApp.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task LogoutAsync();
    Task<UserInfoDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
}

