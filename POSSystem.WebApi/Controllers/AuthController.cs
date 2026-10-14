using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using POSSystem.Application.DTOs;
using POSSystem.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace POSSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly IConfiguration _configuration;


        public AuthController(AccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }


        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            var account = _accountService.Login(request.Username, request.Password);


            if (account == null)
            {
                return Unauthorized("ユーザー名若しくはパスワードが正しくありません。");
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username),
                new Claim("IsEmployee", account.IsEmployee.ToString().ToLower())
            };

            if (account.IsEmployee)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Employee"));
            }

            //従業員のJWTにEmployee付与
            if (account.IsEmployee)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Employee"));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                isEmployee = account.IsEmployee
            });
        }

        [HttpPost("resister")]
        public ActionResult Resister([FromBody] ResisterRequest request)
        {
            try
            {
                _accountService.Resister(request.Username, request.Password);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }

            return Ok(new {message = "会員登録が完了しました。"});
        }
    }
}
