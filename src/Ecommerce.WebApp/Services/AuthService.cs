using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Ecommerce.Shared.DTOs;
using Ecommerce.WebApp.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace Ecommerce.WebApp.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";

    public AuthService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return error ?? new AuthResponseDto(false, string.Empty, string.Empty, string.Empty, string.Empty, "Login failed. Please check your credentials.");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result != null && result.Success && !string.IsNullOrWhiteSpace(result.Token))
        {
            await _localStorage.SetItemAsync(TokenKey, result.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
        }

        return result ?? new AuthResponseDto(false, string.Empty, string.Empty, string.Empty, string.Empty, "Invalid server response.");
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<UserInfoDto?> GetCurrentUserAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserInfoDto>("api/auth/me");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(TokenKey);
        return !string.IsNullOrWhiteSpace(token);
    }
}

