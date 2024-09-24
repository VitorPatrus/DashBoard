using BI.Sistemas.API.Interfaces;
using BI.Sistemas.API.Service;
using BI.Sistemas.API.View;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http.Headers;

namespace BI.Sistemas.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class EconometroController : ControllerBase
    {
        private IEconometroService _econometroService;
        public EconometroController(IEconometroService econometroService)
        {
            _econometroService = econometroService;
        }

        [HttpGet]
        public async Task<ActionResult<EconometroViewModel>> GetEconometroDashboard()
        {
            var econometroData = _econometroService.GetEconometroDashboard();
            return Ok(econometroData);
        }
    }
}
