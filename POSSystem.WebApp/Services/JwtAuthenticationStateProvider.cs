using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace POSSystem.WebApp.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly AuthState _authState;

        public JwtAuthenticationStateProvider(AuthState authState)
        {
            _authState = authState;
            // OnChange イベント発生時に Blazor に状態変化を通知する
            _authState.OnChange += OnAuthStateChanged;
        }

        public void Dispose()
        {
            _authState.OnChange -= OnAuthStateChanged;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrEmpty(_authState.Token))
            {
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                return Task.FromResult(new AuthenticationState(anonymous));
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(_authState.Token);

                var identity = new ClaimsIdentity(jwt.Claims, authenticationType: "jwt");
                var user = new ClaimsPrincipal(identity);

                return Task.FromResult(new AuthenticationState(user));
            }
            catch
            {
                // トークン解析失敗時は未認証扱いにする
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                return Task.FromResult(new AuthenticationState(anonymous));
            }
        }

        private void OnAuthStateChanged()
        {
            // CascadingAuthenticationState に状態が変更されたことを通知する
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
