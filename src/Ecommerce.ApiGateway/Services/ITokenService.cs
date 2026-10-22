using Ecommerce.Shared.DTOs;

namespace Ecommerce.ApiGateway.Services;

public interface ITokenService
{
    string GenerateJwtToken(string email, string role, string customerId);
}

