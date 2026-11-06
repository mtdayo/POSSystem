using POSSystem.WebApp.Models;
using System.Net.Http.Headers;

namespace POSSystem.WebApp.Services
{
    public class AccountApiClient
    {
        private readonly HttpClient _httpClient;

        public AccountApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AccountBalance?> GetBalanceAsync(string token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Account/balance");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);

            if(!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AccountBalance>();
        }
    }
}
