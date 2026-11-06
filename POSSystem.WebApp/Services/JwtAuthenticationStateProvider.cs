using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace POSSystem.WebApp.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        //private readonly AuthState _authState;
        private string? _token;
        public string? Token => _token;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrWhiteSpace(_token))
            {
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            try
            {

                var jwt = new JwtSecurityTokenHandler()
                    .ReadJwtToken(_token);
        
                var identity = new ClaimsIdentity(jwt.Claims, authenticationType: "jwt");
                var user = new ClaimsPrincipal(identity);

                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
            }
            catch
            {
                // トークン解析失敗時は未認証扱いにする
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }
        }

        private void OnAuthStateChanged()
        {
            // CascadingAuthenticationState に状態が変更されたことを通知する
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void SetToken(string token)
        {
            _token = token;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void Logout()
        {
            _token = null;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
