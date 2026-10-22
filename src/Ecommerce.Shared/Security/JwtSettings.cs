namespace Ecommerce.Shared.Security;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = "EcommerceMicroservicesSecretKeyWithAtLeast256BitsLength!";
    public string Issuer { get; set; } = "Ecommerce.ApiGateway";
    public string Audience { get; set; } = "Ecommerce.Clients";
    public int ExpiryMinutes { get; set; } = 120;
}

