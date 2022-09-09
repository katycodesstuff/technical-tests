using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using MeterReadingsUploader.Database.Repositories;

namespace MeterReadingsUploader.Controllers
{
    [ApiController]
    [Route("accounts")]
    [Produces(MediaTypeNames.Application.Json)]
    public class AccountsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromServices]IAccountRepository accountRepository)
        {
            return Ok(new
            {
                Accounts = accountRepository.GetAll()
            });
        }
    }
}
