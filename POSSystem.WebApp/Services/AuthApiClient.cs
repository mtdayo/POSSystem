using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Json;
using POSSystem.WebApp.Models;
using System.Net.WebSockets;

namespace POSSystem.WebApp.Services
{
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<LoginResponse?> LoginAsync(string username, string password)
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

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<(bool Success, string? ErrorMessage)> ResisterAsync(string username, string password)
        {
            var request = new { Username = username, Password = password };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/resister",
                request);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            try
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

                if (error != null && error.TryGetValue("message", out var message))
                {
                    return (false, message);
                }
            }

            catch
            {
                // エラーレスポンスがJSONでない場合
            }

            return (false, "会員登録に失敗しました。入力内容を確認してください。");
        }
    }
}
