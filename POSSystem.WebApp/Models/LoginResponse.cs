namespace POSSystem.WebApp.Models
{
    public class LoginResponse
    {
        public int AccountId { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsEmployee { get; set; }
    }
}
