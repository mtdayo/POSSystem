using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Json;
using POSSystem.WebApp.Models;

namespace POSSystem.WebApp.Services
{
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthState _authState;

        public AuthApiClient(HttpClient httpClient, AuthState authState)
        {
            _httpClient = httpClient;
            _authState = authState;
        }


        public async Task<string?> LoginAsync(string username, string password)
        {
            var request = new Models.LoginRequest { Username = username, Password = password };

            //JWTを取得
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/Login",
                request
            );

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return result?.Token;
        }
    }
}
