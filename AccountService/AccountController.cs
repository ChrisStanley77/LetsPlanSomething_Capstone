using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IResult> CreateAccount(Account account)
        {
            System.Console.WriteLine("Inside the create method.");
            _ACDB.Accounts.Add(account);
            await _ACDB.SaveChangesAsync();
            return Results.Created($"/{account.Username}", account);
        }

//////////////////////////////////////////////// Account Get Endpoints /////////////////////////////////////////////////////////////////////

        //Get an account by email
        [HttpGet("{email}")]
        public async Task<ActionResult<Account>> GetAccount(string email)
        {
            var account = await _ACDB.Accounts.Where(m => m.Email  == email).ToListAsync();
            
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
        public async Task<IResult> UpdateAccount(Account accountUpdateWith)
        {
            var accountToUpdate = await _ACDB.Accounts.FindAsync(accountUpdateWith.Id);

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
        [HttpDelete("{id}")]
        public async Task<IResult> DeleteAccount(string email)
        {
            if(await _ACDB.Accounts.FindAsync(email) is Account account)
            {
                _ACDB.Accounts.Remove(account);
                await _ACDB.SaveChangesAsync();
                return Results.Ok(account);
            }
            
            return Results.NotFound();
        }
    }
}