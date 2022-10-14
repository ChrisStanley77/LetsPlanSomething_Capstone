using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AccountDB _ACDB;

        public UserController(ILogger<UserController> logger, AccountDB acdb)
        {
            _ACDB = acdb;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserCredentails userCred)
        {
            var user = await _ACDB.Accounts.FirstOrDefault(a => a.Username.Equals(userCred.username) && a.Password.Equals(userCred.password));
            if(user)
            {
                
            }
            return Ok();
        }
    }
}