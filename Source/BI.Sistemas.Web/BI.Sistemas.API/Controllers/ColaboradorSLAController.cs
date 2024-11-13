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
using BI.Sistemas.API.Interfaces;
using BI.Sistemas.API.Service;



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

        [HttpPost]
        [Route("SendSLAEmail")]
        public async Task<IActionResult> SendEmail(EnviarDadosSLA dados)
        {
            try
            {
                await _emailService.SendSLAEmail(dados);

                return Ok(new { mensagem = "E-mail enviado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = $"Falha ao enviar o e-mail: {ex.Message}" });
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

                return Ok(new
                {
                    SUP
                });
            }
        }
    }
}

