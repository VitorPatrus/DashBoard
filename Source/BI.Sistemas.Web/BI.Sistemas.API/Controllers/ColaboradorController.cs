using BI.Sistemas.API.Interfaces;
using BI.Sistemas.API.View;
using Microsoft.AspNetCore.Mvc;

namespace BI.Sistemas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColaboradorController : ControllerBase
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IEmailService _emailService;

        public ColaboradorController(IColaboradorService colaboradorService, IEmailService emailService)
        {
            _colaboradorService = colaboradorService;
            _emailService = emailService;
        }

        [HttpGet()]
        [Route("ColaboradorDashboard")]
        public async Task<ActionResult<ColaboradorDashboardView>> GetColaboradorDashboard(string id)
        {
            return Ok(_colaboradorService.GetColaboradorDashboard(id));
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(EnviarEmailDados dados)

        {
            try
            {
                return Ok(_emailService.SendEmailAsync(dados));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
