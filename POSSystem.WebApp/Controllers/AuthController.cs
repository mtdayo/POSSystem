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
            var result = await _authApiClient.LoginAsync(username, password);

            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                return Redirect("/login?error=1");
            }

            //JWTの読み取りだけで認証済みと判断しない
            //APIのログイン成功を前提にcookieの識別情報を作る
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new("api_access_token", result.Token),
                new("IsEmployee", result.IsEmployee.ToString().ToLower())
            };

            if (result.IsEmployee)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Employee"));
            }
            

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

        [HttpPost("resister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resister(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Redirect("/resister?error=empty");
            }

            //パスワードが一致しているかどうか
            if (password != confirmPassword)
            {
                return Redirect("/resister?error=password");
            }

            var result = await _authApiClient.ResisterAsync(username, password);

            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("既に使用") == true)
                {
                    return Redirect("/resister?error=duplicate");
                }

                return Redirect("/resister?error=failed");
            }

            return Redirect("/login?resistered=1");
        }
    }
}
