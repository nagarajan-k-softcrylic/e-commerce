namespace Ecommerce.Shared.DTOs;

public record LoginRequestDto(string Email, string Password);

public record AuthResponseDto(
    bool Success,
    string Token,
    string Email,
    string Role,
    string CustomerId,
    string? Message = null
);

public record UserInfoDto(
    string CustomerId,
    string Email,
    string Role
);

