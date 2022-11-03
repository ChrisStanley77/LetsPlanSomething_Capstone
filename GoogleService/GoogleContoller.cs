using System;

namespace Controllers
{
    [ApiController]
    [Route("google")]
    public class Controller : ControllerBase
    {

        [HttpGet]
        [Route("getPlace")]
        public Task<ActionResult> GetByPlace()
        {
            var key = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

            
        }
    }
}