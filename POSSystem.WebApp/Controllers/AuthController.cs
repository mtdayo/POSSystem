using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using POSSystem.WebApp.Services;

namespace POSSystem.WebApp.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly AuthApiClient _authApiClient;

        public AuthController(AuthApiClient authApiClient)
        {
            _authApiClient = authApiClient;
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var token = await _authApiClient.LoginAsync(username, password);

            if (string.IsNullOrWhiteSpace(token))
            {
                return Redirect("/login?error=1");
            }

            //JWTの読み取りだけで認証済みと判断しない
            //APIのログイン成功を前提にcookieの識別情報を作る
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new("api_access_token", token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect("/Products");


        }

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/login");
        }
    }
}
