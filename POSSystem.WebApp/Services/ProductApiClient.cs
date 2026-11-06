using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using POSSystem.WebApp.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace POSSystem.WebApp.Services
{
    public class ProductApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public ProductApiClient(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;

            //JWTの存在を確認するために、AuthStateのハッシュコードを出力
            //Console.WriteLine($"ProductApiClient側 AuthState: {_authState.GetHashCode()}");
        }


        /// <summary>
        /// Authorizationヘッダーを追加するためのメソッド
        /// </summary>
        /// <param name="request"></param>
        //private void AddAuthorizationHeader(HttpRequestMessage request)
        //{
        //    if (!string.IsNullOrEmpty(_authenticationProvider.Token))
        //    {
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationProvider.Token);
        //    }
        //}


        /// <summary>
        /// 商品一覧を取得するための非同期メソッド
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public async Task<List<Product>> GetProductsAsync(string? search=null, string sortBy ="code", string sortOrder="asc")
        {
            var url =
                $"api/Products" +
                $"?search={Uri.EscapeDataString(search ?? string.Empty)}" +
                $"&sortBy={Uri.EscapeDataString(sortBy)}" +
                $"&sortOrder={Uri.EscapeDataString(sortOrder)}";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Product>>()
                ?? new List<Product>();
        }

        /// <summary>
        /// 商品コードに基づいて商品を取得するための非同期メソッド
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Product?> GetProductByCodeAsync(string code)
        {
            return await _httpClient.GetFromJsonAsync<Product>($"api/Products/{code}");
        }

        /// <summary>
        /// 商品を追加するための非同期メソッド
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> AddProductAsync(AddProductRequest request)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/Products");
            httpRequest.Content = JsonContent.Create(request);

            await AddAuthorizationHeaderAsync(httpRequest);
            return await _httpClient.SendAsync(httpRequest);
        }


        /// <summary>
        /// 商品を更新するための非同期メソッド
        /// </summary>
        /// <param name="code"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateProductAsync(string code, UpdateProductRequest request)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/Products/{code}");

            httpRequest.Content = JsonContent.Create(request);

            // JWTの存在を確認するために、AuthStateのハッシュコードを出力
            //Console.WriteLine($"ProductApiClient側 JWT存在: {!string.IsNullOrEmpty(_authState.Token)}");

            await AddAuthorizationHeaderAsync(httpRequest);

            return await _httpClient.SendAsync(httpRequest);
        }


        /// <summary>
        /// 商品を削除するための非同期メソッド
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteProductAsync(string code)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/Products/{code}");

            await AddAuthorizationHeaderAsync(httpRequest);

            return await _httpClient.SendAsync(httpRequest);
        }

        /// <summary>
        /// JWTを取得するメソッド
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task AddAuthorizationHeaderAsync(HttpRequestMessage request)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var token = user.FindFirst("api_access_token")?.Value;

            if(!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
