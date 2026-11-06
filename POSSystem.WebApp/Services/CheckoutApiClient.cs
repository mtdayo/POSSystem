namespace POSSystem.WebApp.Services
{
    public class CheckoutApiClient
    {
        private readonly HttpClient _httpClient;

        public CheckoutApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
