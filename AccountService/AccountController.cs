using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace Controllers
{
    [Authorize]
    [ApiController]
    [Route("account")]
    public class Controller : ControllerBase
    {
        private readonly AccountDB _ACDB;
        private readonly AppSettings appSettings;
        
        public Controller(ILogger<Controller> logger, AccountDB acdb, IOptions<AppSettings> options)
        {            
            _ACDB = acdb;
            appSettings = options.Value;
        }

//////////////////////////////////////////////// Account Creation Endpoints ////////////////////////////////////////////////////////////////

        //Create an account
        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> CreateAccount(Account account)
        {
            account.Password = HashPassword(account.Password);
            _ACDB.Accounts.Add(account);
            await _ACDB.SaveChangesAsync();
            return Results.Created($"/{account.Username}", account);
        }

        // I think this will be the login part I think....
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate ([FromBody] UserCredentails userCredentails)
        {
            var user = await _ACDB.Accounts.FirstOrDefaultAsync(a => a.Username == userCredentails.username);            
            
            if(user == null)
            {
                return NotFound();
            }
            else if(ValidatePassword(userCredentails.password, user.Password))
            {
                //Generate token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject = new ClaimsIdentity(
                        new Claim[]{new Claim(ClaimTypes.Name, user.Username)}
                    ),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                string finalToken = tokenHandler.WriteToken(token);
                
                return Ok(finalToken);
            }
            else
            {
                return Unauthorized();
            }
        }

//////////////////////////////////////////////// Account Get Endpoints /////////////////////////////////////////////////////////////////////

        //Get an account by email
        [HttpGet]
        [Route("/email/{email}")]
        public async Task<ActionResult<Account>> GetAccount(string email)
        {
            var account = await _ACDB.Accounts.FirstOrDefaultAsync(m => m.Email.Equals(email));
            
            if(account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpGet]
        [Route("test")]
        public ActionResult<String> TestEndPoint(){
            return "Test Succesful";
        }

////////////////////////////////////////////////// Account Update Endpoints ///////////////////////////////////////////////////////////////////

        //Update account by email
        [HttpPut]
        public async Task<IResult> UpdateAccount(Account accountUpdateWith)
        {
            var accountToUpdate = await _ACDB.Accounts.FirstOrDefaultAsync(a => a.Username.Equals(accountUpdateWith.Username));

            if(accountToUpdate == null)
            {
                return Results.NotFound();
            }

            //Update Values
            accountToUpdate.Email = accountUpdateWith.Email;
            accountToUpdate.Firstname = accountUpdateWith.Firstname;
            accountToUpdate.Lastname = accountUpdateWith.Lastname;
            accountToUpdate.Password = accountUpdateWith.Password;

            await _ACDB.SaveChangesAsync();

            return Results.NoContent();
        } 

////////////////////////////////////////////////// Account Delete Endpoints //////////////////////////////////////////////////////////////////

        //Delete account by email
        [HttpDelete("{email}")]
        public async Task<IResult> DeleteAccount(string email)
        {
            if(await _ACDB.Accounts.FirstOrDefaultAsync(a => a.Email.Equals(email)) is Account account)
            {
                _ACDB.Accounts.Remove(account);
                await _ACDB.SaveChangesAsync();
                return Results.Ok(account);
            }
            
            return Results.NotFound();
        }

        public static string GeneratePasswordSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GeneratePasswordSalt());
        }

        public static bool ValidatePassword(string password, string databaseHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, databaseHash);
        }
    }
}