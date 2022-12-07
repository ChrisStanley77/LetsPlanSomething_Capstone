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
        public async Task<IResult> CreateAccount([FromBody] Account account)
        {
            account.Password = HashPassword(account.Password);
            _ACDB.Accounts.Add(account);
            await _ACDB.SaveChangesAsync();
            return Results.Created($"/{account.Username}", account);
        }

        // I think this will be the login part, I think....
        // This also might need to change from a Task<IActionResult> to just a string being returned.
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<string> Authenticate ([FromBody] UserCredentails userCredentails)
        {
            var user = await _ACDB.Accounts.FirstOrDefaultAsync(a => a.Username == userCredentails.username);            
            
            if(user == null)
            {
                //return Results.NotFound();
                return "Account not found";
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

                // UserLoginDTO userDto = new UserLoginDTO();
                // userDto.Firstname = user.Firstname;
                // userDto.Lastname = user.Lastname;
                // userDto.Email = user.Email;
                // userDto.Username = user.Username;
                // userDto.Token = finalToken;
                
                //return Results.Accepted($"/{userDto.Username}", userDto); 
                return finalToken;
            }
            else
            {
                //return Results.Unauthorized();
                return "Username or password incorrect";
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
        [Route("password")]
        public async Task<ActionResult<string>> GetPassword(string username)
        {
            var account = await _ACDB.Accounts.FirstOrDefaultAsync(a => a.Username.Equals(username));

            if(account == null)
            {
                return NotFound();
            }

            return Ok(account.Password);
        }

        [HttpGet]
        [Route("test")]
        public ActionResult<String> TestEndPoint(){
            return "Test Succesful";
        }

////////////////////////////////////////////////// Account Update Endpoints ///////////////////////////////////////////////////////////////////

        //Update account by email
        [HttpPut]
        [Route("updateuser")]
        public async Task<IResult> UpdateAccount(UserDTO userDTO)
        {
            var accountToUpdate = await _ACDB.Accounts.FirstOrDefaultAsync(a => a.Username.Equals(userDTO.Username));

            if(accountToUpdate == null)
            {
                return Results.NotFound();
            }

            if(User?.Identity?.Name == accountToUpdate.Username)
            {
                //Update Values
                accountToUpdate.Email = userDTO.Email;
                accountToUpdate.Firstname = userDTO.Firstname;
                accountToUpdate.Lastname = userDTO.Lastname;

                accountToUpdate.Password = HashPassword(accountToUpdate.Password);

                await _ACDB.SaveChangesAsync();

                return Results.NoContent();
            }
            else
            {
                return Results.Forbid();
            }       
        } 

        [HttpPut]
        [Route("updatepassword")]
        public async Task<IResult> UpdatePassword(UserPasswordUpdate newPassword)
        {
            var accountToUpdate = await _ACDB.Accounts.FirstOrDefaultAsync(a => a.Username.Equals(newPassword.Username));

            if(accountToUpdate == null)
            {
                return Results.NotFound();
            }
            else if(User?.Identity?.Name == accountToUpdate.Username)
            {
                //Update Values
                accountToUpdate.Password = newPassword.Password;

                accountToUpdate.Password = HashPassword(accountToUpdate.Password);

                await _ACDB.SaveChangesAsync();

                return Results.NoContent();
            }
            else 
            {
                return Results.Forbid();
            }
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