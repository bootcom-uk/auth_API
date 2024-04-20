using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Authorize]
    [Route("/[controller]")]    
    public class BaseController : ControllerBase
    {

        internal readonly IDatabaseService _databaseService;

        internal readonly IConfiguration _configuration;

        public BaseController(IDatabaseService databaseService, IConfiguration configuration) { 
            _databaseService = databaseService;
            _configuration = configuration;
        }

    }
}
