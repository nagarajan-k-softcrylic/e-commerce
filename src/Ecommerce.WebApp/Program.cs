using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Ecommerce.WebApp;
using Ecommerce.WebApp.Authentication;
using Ecommerce.WebApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Blazorise & UI Providers
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Authentication & Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Configure HTTP Client pointing to API Gateway (default: http://localhost:5000)
var apiGatewayUrl = builder.Configuration["ApiGatewayBaseUrl"] ?? "http://localhost:5000";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiGatewayUrl.EndsWith("/") ? apiGatewayUrl : apiGatewayUrl + "/")
});

// Application Business Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<CartService>();

var host = builder.Build();
await host.RunAsync();

