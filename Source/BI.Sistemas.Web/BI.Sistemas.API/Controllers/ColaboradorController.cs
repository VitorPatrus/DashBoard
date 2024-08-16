using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static BI.Sistemas.API.View.ColaboradorDashboardView;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace BI.Sistemas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColaboradorController : ControllerBase
    {
        [HttpGet()]
        [Route("ColaboradorDashboard")]
        public ActionResult<ColaboradorDashboardView> GetColaboradorDashboard(string id)
        {
            
            TMetric metricas = new TMetric();
            if (id.IsNullOrEmpty())
                return BadRequest("O ID do colaborador não foi fornecido!");

            using (var db = new BISistemasContext())
            {
                var pontosTodos = db.Pontos.ToList();
                var tmetricTodos = db.TMetrics.Where(a =>
                a.Usuario != "Marco Aurelio de Barros" &&
                a.Usuario != "Andre Costa (TI MTZ)" &&
                a.Usuario != "Marco Tulio Rodrigues").ToList();

                var pessoa = db.Colaboradores.FirstOrDefault(c => c.Id.ToString() == id);

                if (pessoa == null)
                    return NotFound($"Colaborador não encontrado (ID: {id})");

                var colaborador = new ColaboradorDashboardView();

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                DirectoryInfo projectDirectory = null;
                if (baseDirectory != null)
                    projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.Parent.Parent;


                colaborador.Nome = pessoa.Nome;
                colaborador.Email = pessoa.Email;
                colaborador.FotoColaborador = Convert.ToBase64String(pessoa.Foto);
                colaborador.Cargo = pessoa.Cargo;
                colaborador.Time = $"Time {pessoa.Time}";
                var periodoAtual = db.Periodos.OrderBy(x => x.Data).Last();
                 colaborador.FotoTime = Convert.ToBase64String(System.IO.File
                    .ReadAllBytes($@"{projectDirectory}\UI\Content\Images\time-{pessoa.Time}.png"));

                var HE = pontosTodos
                .Where(he => he.PeriodoId.ToString().ToUpper() == periodoAtual.Id.ToString().ToUpper())
                .LastOrDefault();

                var paulo = db.Colaboradores.ToList();
                string teste = paulo.Count > 0 ? paulo[1].Id.ToString() : "a";

                if (teste.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                {
                    var a = 1;
                    var b = 2;
                    var c = a + b;
                }

                var HoraPonto = pontosTodos
                    .Where(he => he.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper() && he.ColaboradorId.ToString().Equals(id, StringComparison.OrdinalIgnoreCase) /*he.ColaboradorId.ToString().ToUpper() == id.ToUpper()*/ && he.Tipo == TipoPonto.Normal)
                    .Select(C => C.Horas);

                if (HoraPonto != null && HoraPonto.Any())
                {
                    colaborador.TotalPonto = metricas.GetHoras(HoraPonto);
                }
                else
                {
                    colaborador.TotalPonto = pessoa.CargaHoraria;
                }

                var horas = tmetricTodos
                    .Where(tm => tm.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper() && tm.ColaboradorId.ToString().ToUpper() == id)
                    .ToList();

                var metrics = tmetricTodos.Where(m => m.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper()).ToList();

                var pontos = pontosTodos
                    .Where(p => p.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper() && p.Tipo == TipoPonto.Normal).ToList();

                var excludes = new string[]
                {
                    "010A52D3-D812-4DCE-D721-08DC62DFD032",
                    "D2E3452C-E900-4573-D724-08DC62DFD032",
                    "22654802-4100-4CF7-D727-08DC62DFD032"
                };

                var engajamentos = new Dictionary<Colaborador, int>();
                foreach (var col in db.Colaboradores.ToList()
                    .Where(c => c.Time == "TMS" || c.Time == "ERP")
                    .Where(c => !excludes.Contains(c.Id.ToString().ToUpper()))
                    .Where(c => c.CargaHoraria > 0 || pontos.Exists(p => p.ColaboradorId?
                    .ToString().ToUpper() == c.Id.ToString().ToUpper()))
                    .ToList())
                    engajamentos.Add(col, metricas.CalcularEngajamento(metrics, pontos, col));

                var engajamentoTime = engajamentos.Where(e => e.Key.Time == pessoa.Time).ToList();

                colaborador.Engajamento = engajamentos.FirstOrDefault(e => e.Key.Id.ToString().ToUpper() == pessoa.Id.ToString().ToUpper()).Value;
                colaborador.EngajamentoTime = engajamentoTime.Sum(t => t.Value) / engajamentoTime.Count();

                colaborador.TopEngajamento = engajamentos.Where(x => x.Value <= 100).OrderByDescending(e => e.Value).Take(3)
                    .Select(e =>
                    new EngajamentoView()
                    {
                        Nome = e.Key.Nome,
                        Foto = Convert.ToBase64String(e.Key.Foto),
                        Percentual = e.Value
                    })
                    .ToArray();

                colaborador.Atividades = metrics.Where(a => a.ColaboradorId.ToString().ToUpper() == pessoa.Id.ToString().ToUpper()).OrderBy(a => a.Data)
                .Select(a =>
                    new AtividadeView()
                    {
                        Data = a.Data.ToString(),
                        Atividade = a.Atividade,
                        Horas = a.Duracao,
                        Ticket = a.DevopsTask.ToString(),
                        Tipo = a.Tipo
                    })
                .ToArray();


                var heTime = new Dictionary<string, string>
                {
                    { "TMS", "00:03" },
                    { "ERP", "00:13" }
                };
                colaborador.HE_Equipe = heTime[pessoa.Time];
                colaborador.PJ = pessoa.CargaHoraria > 0;

                AddEngajamento(id, colaborador);
                return Ok(colaborador);
            }
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(EnviarEmailDados dados)
        {
            EnviarEmailDados email = new EnviarEmailDados();

            using (var db = new BISistemasContext())
            {
                var pessoa = db.Colaboradores.FirstOrDefault(p => p.Id.ToString() == dados.Id);
                if (pessoa == null)
                    return NotFound("Arquivo não localizado ou duplicado");

                try
                {
                    Outlook.Application outlookApp = new Outlook.Application();
                    Outlook.MailItem mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

                    if (dados.Oficial)
                    {
                        var coordenador = pessoa.Time == "TMS" ? "tulio@patrus.com.br" : "marco.barros@patrus.com.br";
                        mailItem.CC = $"rogerio.simoes@patrus.com.br;{coordenador}";
                        mailItem.To = pessoa.Email;
                    }
                    else
                    {
                        mailItem.To = "vitor.fernandessouza@patrus.com.br";
                    }

                    var tabelaAtividade = @"
                    <table style='border-collapse: collapse; width: 100%;'>
                        <thead>
                            <tr>
                                <th style='border: 1px solid #000; padding: 8px;'>Atividade</th>
                                <th style='border: 1px solid #000; padding: 8px;'>Ticket</th>
                                <th style='border: 1px solid #000; padding: 8px;'>Tipo</th>
                                <th style='border: 1px solid #000; padding: 8px;'>Data</th>
                                <th style='border: 1px solid #000; padding: 8px;'>Horas</th>
                            </tr>
                        </thead>
                        <tbody>
                            " + string.Join("", dados.Lista.Select(c => $@"
                            <tr>
                                <td style='border: 1px solid #000; padding: 8px;'>{c.Atividade}</td>
                                <td style='border: 1px solid #000; padding: 8px;'>{c.Ticket}</td>
                                <td style='border: 1px solid #000; padding: 8px;'>{c.Tipo}</td>
                                <td style='border: 1px solid #000; padding: 8px;'>{c.Data.Substring(0, 10)}</td>
                                <td style='border: 1px solid #000; padding: 8px;'>{c.Horas.ToString("HH:mm")}</td>
                            </tr>")) + @"
                        </tbody>
                    </table>";

                    string gif = string.Empty;
                    string mensagem = string.Empty;
                    var engajamentoDevOps = dados.DevOps == 0 ? "0,0" : string.Format("{0:#.#,##}", Math.Round(dados.DevOps, 1));
                    if (dados.Engajamento > 100)
                    {
                        gif = Gifs.ListasGifs(NivelGif.Cinquenta);
                        mensagem = $@"Seu engajamento foi de <b>{dados.Engajamento}% e lançamentos pelo Devops de {engajamentoDevOps}%. Você registrou um tempo de trabalho superior em relação ao ponto.</b> Atenção com os lançamentos.";
                    }
                    else if (dados.Engajamento >= 90 && dados.DevOps >= 95)
                    {
                        mensagem = $"Seus lançamentos estão excelentes, <b>parabéns</b>✅. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                        gif = Gifs.ListasGifs(NivelGif.Noventa);
                    }
                    else if (dados.Engajamento >= 0 && dados.DevOps >= 95)
                    {
                        mensagem = $"<b>Atenção com os lançamentos no dia à dia</b>⚠️. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                        gif = Gifs.ListasGifs(NivelGif.Cinquenta);
                    }
                    else if (dados.Engajamento >= 90 && dados.DevOps < 95)
                    {
                        mensagem = $"<b>Atenção com os lançamentos dentro das atividades do Devops</b>⚠️. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                        gif = Gifs.ListasGifs(NivelGif.Cinquenta);
                    }
                    else if (dados.Engajamento < 90 && dados.DevOps < 95)
                    {
                        mensagem = $"<b>Seus lançamentos não estão legais</b>, mais atenção com os lançamentos do dia à dia e dentro das atividades do Devops ❌. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                        gif = Gifs.ListasGifs(NivelGif.VinteCinco);
                    }
                    mailItem.Subject = $@"[SISTEMAS] APURAÇÃO DE HORAS SEMANAL - {pessoa.Nome.ToUpper()} ";
                    mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                    string msgHTMLBody = $@"
                     <html>
                        <head></head>
                        <body>
                            {email.MensagemInicial()}, {pessoa.Nome},<br><br>
                            Segue o dashboard com os lançamentos do período.<b> {dados.Periodo}.</b><br><br>
                            <div style=""min-width:600px!important;margin-left:auto;margin-right:auto; ""></div>
                            <img src=""{gif}"" width=""80"" height=""80""/><br>
                            {mensagem}<br><br>
                            <img align=""baseline"" border=""1"" hspace=""0"" src=""{dados.Foto}"" width=""880"" 600="""" hold="" /> ""></img><br>
                            <i>
                            <br><br>
                            Segue abaixo a tabela com os lançamentos individuais do TMETRIC:
                            <br><br><hr /><br>
                            <b>Atividades trabalhadas</b><br>
                            <i>Atividades realizadas durante o período {dados.Periodo}</i>
                            <br><br>
                            {tabelaAtividade}
                            <br><br>
                            Caso encontre alguma inconsistência em relação às informações contidas neste e-mail, favor nos contatar.<br><br>
                            Atenciosamente,<br>
                            Vitor Fernandes.
                        </body>
                    </html>";

                    mailItem.HTMLBody = msgHTMLBody;
                    mailItem.Send();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        private void Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        private List<EvolucaoEngajamentoView> AddEngajamento(string id, ColaboradorDashboardView colaborador)
        {
            var anterior1 = 0; var anterior2 = 0; var anterior3 = 0; var anterior4 = 0; var anterior5 = 0;

            switch (id)
            {
                case "69DB13EF-89C0-4A6F-D71F-08DC62DFD032": // Amanda Ferreira
                    colaborador.HE_Individual = "00:11";
                    anterior3 = 96;
                    anterior4 = 97;
                    anterior5 = 94;
                    anterior1 = 0;
                    anterior2 = 0;  
                    break;

                case "BD984996-9C11-4095-D71D-08DC62DFD032": // Fernanda Cassiano
                    colaborador.HE_Individual = colaborador.TotalPonto.ToString();
                    anterior3 = 87;
                    anterior4 = 93;
                    anterior5 = 94;
                    anterior1 = 95;
                    anterior2 = 95;
                    break;

                case "52F14677-9C85-41D5-D723-08DC62DFD032": // João Paulo
                    colaborador.HE_Individual = colaborador.TotalPonto.ToString();
                    anterior3 = 98;
                    anterior4 = 98;
                    anterior5 = 91;
                    anterior1 = 99;
                    anterior2 = 92;
                    break;

                case "C44D7319-3318-43D4-D726-08DC62DFD032": // Joel Martins
                    colaborador.HE_Individual = "-00:02";
                    anterior3 = 94;
                    anterior4 = 94;
                    anterior5 = 90;
                    anterior1 = 91;
                    anterior2 = 87;
                    break;

                case "3F7E1A71-815A-4397-D725-08DC62DFD032": // Junior Dias
                    colaborador.HE_Individual = "00:02";
                    anterior3 = 99;
                    anterior4 = 104;
                    anterior5 = 86;
                    anterior1 = 90;
                    anterior2 = 100;
                    break;

                case "0D6227A1-7B72-4DAC-D720-08DC62DFD032": // Luiz Oliveira
                    colaborador.HE_Individual = colaborador.TotalPonto.ToString();
                    anterior3 = 100;
                    anterior4 = 60;
                    anterior5 = 58;
                    anterior1 = 97;
                    anterior2 = 100;
                    break;

                case "C0D4394F-38EF-4F8B-D71E-08DC62DFD032": // Paulo Silva
                    colaborador.HE_Individual = "-00:01";
                    anterior3 = 95;
                    anterior4 = 94;
                    anterior5 = 95;
                    anterior1 = 97;
                    anterior2 = 97;
                    break;

                case "87B833CD-7810-4030-D722-08DC62DFD032": // Thiago Oliveira
                    colaborador.HE_Individual = "00:02";
                    anterior3 = 96;
                    anterior4 = 95;
                    anterior5 = 95;
                    anterior1 = 94;
                    anterior2 = 94;
                    break;

                case "11B207E8-E5F6-44B6-32CA-08DC9125DFEC": // Petrônio Aleixo
                    colaborador.HE_Individual = "00:04";
                    anterior3 = 90;
                    anterior4 = 97;
                    anterior5 = 93;
                    anterior1 = 91;
                    anterior2 = 92;
                    break;
            }

            var lista = new List<EvolucaoEngajamentoView>();

            lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(42,41), Valor = anterior3 });
            lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(35,34), Valor = anterior4 });
            lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(28,27), Valor = anterior5 });
            lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(21,20), Valor = anterior1 });
            lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(14,13), Valor = anterior2 });
            lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(7,6),   Valor = colaborador.Engajamento });

            colaborador.EvolucaoEngajamento = lista.ToArray();

            return lista;
        }
    }
}

