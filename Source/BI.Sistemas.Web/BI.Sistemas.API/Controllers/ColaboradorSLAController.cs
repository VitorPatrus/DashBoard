using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using Microsoft.AspNetCore.Mvc;
using BI.Sistemas.API.Interfaces;


namespace BI.Sistemas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColaboradorSLAController : ControllerBase
    {
        private IColaboradorSLAService _colaboradorSLAService;
        private readonly IEmailService _emailService;
        public ColaboradorSLAController(IColaboradorSLAService colaboradorSLAService, IEmailService emailService)
        {
            _colaboradorSLAService = colaboradorSLAService;
            _emailService = emailService;
        }

        [HttpGet()]
        [Route("ColaboradorSLADashboard")]
        public async Task<ActionResult<ColaboradorSLADashboardView>> GetColaboradorDashboard(string id)
        {
            return Ok(_colaboradorSLAService.GetColaboradorDashboard(id));
        }

        [HttpGet()]
        [Route("SendSLAEmail")]
        public async Task<IActionResult> SendSLAEmail(EnviarEmailDados dados)
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

        [HttpGet]
        [Route("GetColaboradores")]
        public async Task<IActionResult> GetColaboradores()
        {
            using (var db = new BISistemasContext())
            {
                var colaboradores = db.Colaboradores.ToList();

                var SUP = colaboradores.Where(c => c.Suporte)
                    .Select(x => new
                    {
                        idColaborador = x.Id,
                        NomeColab = x.UserTMetric,
                        FotoColab = x.Foto,
                        TimeColab = x.Time

                    })
                    .OrderBy(x => x.NomeColab)
                    .ToList();

                return Ok(new{SUP});
            }
        }
    }
}

