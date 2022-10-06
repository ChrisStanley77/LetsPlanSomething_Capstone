using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("account")]
    public class Controller : ControllerBase
    {
        private readonly AccountDB _ACDB;
        
        public Controller(ILogger<Controller> logger, AccountDB acdb)
        {            
            _ACDB = acdb;
        }

//////////////////////////////////////////////// Account Creation Endpoints ////////////////////////////////////////////////////////////////

        //Create an account
        [HttpPost]
        public async Task<IResult> PostAccount(Account account)
        {
            _ACDB.Accounts.Add(account);
            await _ACDB.SaveChangesAsync();
            return Results.Created($"/{account.Username}", account);
        }
    }
}