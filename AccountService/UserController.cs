using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AccountDB _ACDB;
        private readonly JwtSettings jwtSettings;

        public UserController(ILogger<UserController> logger, AccountDB acdb, IOptions<JwtSettings> options)
        {
            _ACDB = acdb;
            jwtSettings = options.Value;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(Account userCred)
        {
            var user = await _ACDB.Accounts.FirstOrDefault(a => a.Username.Equals(userCred.Username) && a.Password.Equals(userCred.Password));
            if(user == null)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(jwtSettings.securitykey);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(
                    new Claim[] {new Claim{ClaimTypes.Name, user.Username}}
                ),
                Expires = DateTime.Now.AddSeconds(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string finalToken = tokenHandler.WriteToken(token);

            return Ok(finalToken);
        }
    }
}