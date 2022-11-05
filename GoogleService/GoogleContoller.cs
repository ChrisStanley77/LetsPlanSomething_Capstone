using System;
using Microsoft.AspNetCore.Mvc;



namespace Controllers
{
    [ApiController]
    [Route("google")]
    public class Controller : ControllerBase
    {

        [HttpGet]
        [Route("getPlace")]
        public string GetByPlace()
        {
            var key = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

            return "";
        }
    }
}