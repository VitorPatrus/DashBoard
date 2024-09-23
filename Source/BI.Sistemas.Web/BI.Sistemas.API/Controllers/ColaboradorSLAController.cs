using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Outlook = Microsoft.Office.Interop.Outlook;
using static BI.Sistemas.API.View.ColaboradorDashboardView;
using static BI.Sistemas.API.View.ColaboradorSLADashboardView;
using Microsoft.EntityFrameworkCore;
using BI.Sistemas.Domain.Extensions;
using BI.Sistemas.Domain.Entities;
using BI.Sistemas.Domain.Entities.Enums;
using System.Diagnostics;
using System;
using System.ComponentModel;
using OfficeOpenXml;
using Microsoft.Data.SqlClient;
using LicenseContext = System.ComponentModel.LicenseContext;


namespace BI.Sistemas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColaboradorSLAController : ControllerBase
    {
        private IColaboradorSLAService _colaboradorSLAService;
        public ColaboradorSLAController(IColaboradorSLAService colaboradorSLAService)
        {
            _colaboradorSLAService = colaboradorSLAService;
        }

        [HttpGet()]
        [Route("ColaboradorSLADashboard")]
        public async Task<ActionResult<ColaboradorSLADashboardView>> GetColaboradorDashboard(string id)
        {
            return Ok(_colaboradorSLAService.GetColaboradorDashboard(id));
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

                return Ok(new
                {
                    SUP
                });
            }
        }
    }
}

