using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class DateTimeController : BaseController
    {
        public DateTimeController(IDatabaseService databaseService, IConfiguration configuration) : base(databaseService, configuration)
        {
        }

        [HttpGet]
        public ActionResult GetDateTime()
        {
            return Ok(DateTime.Now);
        }

    }
}
