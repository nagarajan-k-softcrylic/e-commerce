using System.Security.Claims;
using Ecommerce.ApiGateway.Services;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new AuthResponseDto(false, string.Empty, string.Empty, string.Empty, string.Empty, "Email and Password are required."));
        }

        string role;
        string customerId;

        // Verify credentials
        if (request.Email.Equals("admin@ecommerce.com", StringComparison.OrdinalIgnoreCase) && request.Password == "Admin123!")
        {
            role = Roles.Admin;
            customerId = "admin-user-01";
        }
        else if (request.Email.Equals("customer@ecommerce.com", StringComparison.OrdinalIgnoreCase) && request.Password == "Customer123!")
        {
            role = Roles.Customer;
            customerId = "customer-user-01";
        }
        else if (request.Password == "Password123!") // Flexible learning mode
        {
            role = request.Email.Contains("admin", StringComparison.OrdinalIgnoreCase) ? Roles.Admin : Roles.Customer;
            customerId = Guid.NewGuid().ToString("N")[..8];
        }
        else
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
            return Unauthorized(new AuthResponseDto(false, string.Empty, string.Empty, string.Empty, string.Empty, "Invalid email or password. Use demo credentials (Admin: admin@ecommerce.com / Admin123!, Customer: customer@ecommerce.com / Customer123!)"));
        }

        var token = _tokenService.GenerateJwtToken(request.Email, role, customerId);
        _logger.LogInformation("User {Email} logged in successfully with role {Role}.", request.Email, role);

        return Ok(new AuthResponseDto(true, token, request.Email, role, customerId));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name) ?? "unknown";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? Roles.Customer;
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

        return Ok(new UserInfoDto(customerId, email, role));
    }
}

