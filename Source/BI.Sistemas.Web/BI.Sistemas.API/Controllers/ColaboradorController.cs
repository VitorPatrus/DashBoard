﻿using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using static BI.Sistemas.API.View.ColaboradorDashboardView;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace BI.Sistemas.API.Controllers
{
    // Esta classe é responsável por lidar com as solicitações relacionadas aos colaboradores
    [ApiController]
    [Route("[controller]")]
    public class ColaboradorController : ControllerBase
    {
        // Este método retorna o painel do colaborador com base no ID fornecido
        [HttpGet()]
        [Route("ColaboradorDashboard")]
        public ActionResult<ColaboradorDashboardView> GetColaboradorDashboard(string id)
        {
            // Verifica se o ID está presente e não está vazio
            if (string.IsNullOrWhiteSpace(id))
            {
                // Se o ID estiver faltando ou vazio, retorna um erro 400 indicando isso
                return BadRequest("O ID do colaborador não foi fornecido!");
            }

            // Inicia uma instância do contexto do banco de dados
            using (var db = new BISistemasContext())
            {
                /*
                    Se o codigo não e mais ultilizado não tem necessidade de guardar ele 
                    Isso polui a leitura 
                
                */
                var pontosTodos = db.Pontos.ToList();
                var tmetricTodos = db.TMetrics.Where(a =>
                a.Usuario != "Marco Aurelio de Barros" &&
                a.Usuario != "Andre Costa (TI MTZ)" &&
                a.Usuario != "Marco Tulio Rodrigues").ToList();
                // Busca o colaborador no banco de dados com base no ID fornecido
                var pessoa = db.Colaboradores.FirstOrDefault(c => c.Id.ToString() == id);


                // Se o colaborador não for encontrado, retorna um erro 404 indicando isso
                if (pessoa == null)
                {
                    return NotFound($"Colaborador não encontrado (ID: {id})");
                }
                /*
                     Isso daqui pode ser feito em um outro metodo 
                     melhorar espaçamento inutil
                */
                // Cria um objeto para armazenar as informações do painel do colaborador
                var colaborador = new ColaboradorDashboardView();

                // Obtém o diretório base onde o executável está rodando
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                DirectoryInfo projectDirectory = null;
                if (baseDirectory != null)
                    projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.Parent.Parent;

                // Define o nome, a foto e a foto do time do colaborador no objeto do painel
                colaborador.Nome = pessoa.Nome;
                colaborador.Email = pessoa.Email;
                colaborador.FotoColaborador = Convert.ToBase64String(pessoa.Foto);
                colaborador.FotoTime = Convert.ToBase64String(System.IO.File.ReadAllBytes($@"{projectDirectory}\UI\Content\Images\time-{pessoa.Time}.jpg"));
                colaborador.Cargo = pessoa.Cargo;
                colaborador.Time = $"Time {pessoa.Time}";
                var periodoAtual = db.Periodos.OrderBy(x => x.Data).Last();
                var HE = pontosTodos
                .Where(he => he.PeriodoId.ToString().ToUpper() == periodoAtual.Id.ToString().ToUpper())
                .LastOrDefault();

                // Busca o último registro de horas extras do colaborador no banco de dados
                //var HE = db.Pontos
                //    .Where(he => he.ColaboradorId.ToString() == id && he.Tipo == TipoPonto.HE)
                //    .OrderBy(he => he.PeriodoId)
                //    .LastOrDefault();

                //// Se houver registro de horas extras, adiciona ao objeto do painel
                //if (HE != null)
                //{
                //    colaborador.HE_Individual = HE.Horas.ToString("HH:mm");
                //}

                // Busca e formata as horas extras da equipe do colaborador
                //var HE_Time = GetHoras(db.Pontos
                //    .Where(he_time => he_time.PeriodoId.ToString() == HE.PeriodoId.ToString() && he_time.Tipo == TipoPonto.HE)
                //    .ToList()
                //    .Select(he => he.Horas)
                //    );

                // Se houver horas extras da equipe, adiciona ao objeto do painel
                //if (HE_Time != null)
                //{
                //    colaborador.HE_Equipe = $"{HE_Time}:00";
                //}

                // Busca o último registro de ponto normal do colaborador no banco de dados


                // Maneira de comparar strings como melhor performance 

                var paulo = db.Colaboradores.ToList();
                //"C0D4394F-38EF-4F8B-D71E-08DC62DFD032"
                //"c0d4394f-38ef-4f8b-d71e-08dc62dfd032"


                // string teste = paulo.Count > 0 ? paulo[1].Id.ToString() : "a";

                string teste;
                if (paulo.Count > 0)
                {
                    teste = paulo[1].Id.ToString();
                }
                else
                {
                    teste = "a";
                }

                if (teste.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                {
                    var a = 1;
                    var b = 2;
                    var c = a + b;
                }

                var HoraPonto = pontosTodos
                    .Where(he => he.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper() && he.ColaboradorId.ToString().Equals(id, StringComparison.OrdinalIgnoreCase) /*he.ColaboradorId.ToString().ToUpper() == id.ToUpper()*/ && he.Tipo == TipoPonto.Normal)
                    .Select(C => C.Horas);


                // Se houver registro de ponto normal, atualiza o total de pontos no objeto do painel
                if (HoraPonto != null && HoraPonto.Any())
                {
                    colaborador.TotalPonto = GetHoras(HoraPonto);
                }
                else
                {
                    colaborador.TotalPonto = pessoa.CargaHoraria;
                }

                // Busca e calcula o total de horas apropriadas com base nas métricas de tempo do colaborador
                var horas = tmetricTodos
                    .Where(tm => tm.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper() && tm.ColaboradorId.ToString().ToUpper() == id)
                    .ToList();

                // Pegando as métricas do período do histórico de engajamento
                var metrics = tmetricTodos.Where(m => m.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper()).ToList();

                // Pegando os pontos normais do mesmo período
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

                    .Where(c => c.CargaHoraria > 0 || pontos.Exists(p => p.ColaboradorId?.ToString().ToUpper() == c.Id.ToString().ToUpper()))
                    .ToList())
                    engajamentos.Add(col, CalcularEngajamento(metrics, pontos, col));
                var engajamentoTime = engajamentos.Where(e => e.Key.Time == pessoa.Time).ToList();

                // Definindo o engajamento do colaborador específico
                colaborador.Engajamento = engajamentos.FirstOrDefault(e => e.Key.Id.ToString().ToUpper() == pessoa.Id.ToString().ToUpper()).Value;
                colaborador.EngajamentoTime = engajamentoTime.Sum(t => t.Value) / engajamentoTime.Count();

                // Listando os top 3 colaboradores mais engajados
                colaborador.TopEngajamento = engajamentos.Where(x => x.Value <= 100).OrderByDescending(e => e.Value).Take(3)
                    .Select(e =>
                    new EngajamentoView()
                    {
                        Nome = e.Key.Nome,
                        Foto = Convert.ToBase64String(e.Key.Foto),
                        Percentual = e.Value
                    }).ToArray();

                // Pegando as atividades do colaborador específico e ordenando por data
                colaborador.Atividades = metrics.Where(a => a.ColaboradorId.ToString().ToUpper() == pessoa.Id.ToString().ToUpper()).OrderBy(a => a.Data)
                .Select(a =>
                    new AtividadeView()
                    {
                        Data = a.Data.ToString(), //data no lugar da data
                        Atividade = a.Atividade, // atividade no lugar de atividade
                        Horas = a.Duracao,// horas no lugar de duração 
                        Ticket = a.DevopsTask.ToString(),//ticket no lugar das tasks
                        Tipo = a.Tipo// Tipo no lugar de tipo


                        // TicketLink = a.TicketLink.ToString(), // TicketLink no lugar de TicketLink
                    }).ToArray();

                colaborador.PJ = pessoa.CargaHoraria > 0;
                if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                {
                    colaborador.HE_Individual = "44:00";
                }
                else if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                {
                    colaborador.HE_Individual = "01:43";

                }
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                {
                    colaborador.HE_Individual = "44:00";

                }
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                {
                    colaborador.HE_Individual = "09:09";

                }
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                {
                    colaborador.HE_Individual = "02:51";

                }
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                {
                    colaborador.HE_Individual = "35:00";

                }
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                {
                    colaborador.HE_Individual = "00:01";

                }
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                {
                    colaborador.HE_Individual = "00:03";
                }
                else if (id == "11B207E8-E5F6-44B6-32CA-08DC9125DFEC") // Petrônio Faleixo TMS
                {
                    colaborador.HE_Individual = "03:39";
                }

                var heTime = new Dictionary<string, string>
                {
                    { "TMS", "15:40" },
                    { "ERP", "01:46" }
                };
                colaborador.HE_Equipe = heTime[pessoa.Time];



                var anterior3 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior3 = 96;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior3 = 94;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior3 = 93;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior3 = 94;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior3 = 97;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior3 = 100;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior3 = 90;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior3 = 99;

                var anterior4 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior4 = 97;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior4 = 100;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior4 = 100;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior4 = 95;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior4 = 97;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior4 = 100;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior4 = 96;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior4 = 96;


                var anterior5 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior5 = 99;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior5 = 100;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior5 = 100;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior5 = 71;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior5 = 95;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior5 = 100;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior5 = 94;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior5 = 96;
                else if (id == "11B207E8-E5F6-44B6-32CA-08DC9125DFEC") // Petrônio Faleixo ERP
                    anterior5 = 97;

                var anterior6 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior6 = 96;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior6 = 92;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior6 = 98;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior6 = 91;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior6 = 99;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior6 = 78;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior6 = 97;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior6 = 95;
                else if (id == "11B207E8-E5F6-44B6-32CA-08DC9125DFEC") // Petrônio Faleixo ERP
                    anterior6 = 91;

                var anterior7 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior7 = 97;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior7 = 87;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior7 = 98;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior7 = 94;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior7 = 104;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior7 = 100;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior7 = 95;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior7 = 95;
                else if (id == "11B207E8-E5F6-44B6-32CA-08DC9125DFEC") // Petrônio Faleixo ERP
                    anterior7 = 90;

                var anterior8 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior8 = 97;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior8 = 93;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior8 = 98;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior8 = 94;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior8 = 94;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior8 = 60;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior8 = 94;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior8 = 94;
                else if (id == "11B207E8-E5F6-44B6-32CA-08DC9125DFEC") // Petrônio Faleixo ERP
                    anterior8 = 97;





                var lista = new List<EvolucaoEngajamentoView>();



                //lista.Add(new EvolucaoEngajamentoView() { Data = "03/06", Valor = anterior3 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "10/06", Valor = anterior4 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "17/06", Valor = anterior5 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "24/06", Valor = anterior6 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "01/07", Valor = anterior7 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "08/07", Valor = anterior8 });
                DateTime dataAtual = DateTime.Now;
                DayOfWeek diaAtual = dataAtual.DayOfWeek;
                int diasParaSubtrair = (int)diaAtual + 6;
                DateTime segundaFeiraPassada = dataAtual.AddDays(-diasParaSubtrair);
                string segundaFormatada = segundaFeiraPassada.ToString("dd/MM");
                lista.Add(new EvolucaoEngajamentoView() { Data = segundaFormatada, Valor = colaborador.Engajamento });


                colaborador.EvolucaoEngajamento = lista.ToArray();
                return Ok(colaborador);

            }
        }

        public class EnviarEmailDados
        {
            public string Foto { get; set; }
            public string Id { get; set; }
            public string Periodo { get; set; }
            public int Engajamento { get; set; }
            public double DevOps { get; set; }
            public bool Oficial { get; set; }
            public AtividadeView[] Lista { get; set; }
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(EnviarEmailDados dados)
        {
            using (var db = new BISistemasContext())
            {
                var pessoa = db.Colaboradores.FirstOrDefault(p => p.Id.ToString() == dados.Id);
                if (pessoa == null)
                {
                    return NotFound("Arquivo não localizado ou duplicado");
                }

                try
                {
                    Outlook.Application outlookApp = new Outlook.Application();
                    Outlook.MailItem mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

                    // Configuração do destinatário do e-mail
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

                    // Construção da tabela de atividades
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

                    // Configuração da mensagem do e-mail com base no engajamento
                    var ListaGifs90 = new List<string>
                    {
                    "https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExeHR2dzdndzZvOXZwbGl0bzAxeTQ4bWtjczE3Z3EyemtoMnJ3bTdhMCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/130do0lQXXcsla/giphy.gif",
                    "https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExOTU5emNoeXZxOXRmMDRybjR1NjZ0MzdpNGZkaW1lbHJrYXBmMWRvYSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/xT5LMHxhOfscxPfIfm/giphy.gif",
                    "https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExMWM1NWlld2hnZDA2ZThuMG9rdWJrNTFpajV6amxrbnNwNXQ0MzN3MCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/kEKcOWl8RMLde/giphy.gif",
                    "https://media1.giphy.com/media/v1.Y2lkPTc5MGI3NjExb3N4ODI5NnR4MXV0bjFvcmNpdnp2bXFpZ3FwYTk4b2RkdmUyMHB6ayZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/aQwvKKi4Lv3t63nZl9/giphy.gif"

                    };
                    var ListaGifs50 = new List<string>
                    {
                    "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExdDlpZWQxc3liNTExOTl4NjJiMWM2dWZjZGdqODNldHg4aTVhejUzYiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/eNTxLwTGW7E64/giphy.gif",
                    "https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExbjBkbGk5NjNza216OW93YWR6Z29neGZjeHMxYzdpYjdrenpwd29nNSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/H5C8CevNMbpBqNqFjl/giphy.gif",
                    "https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExdGFrZXdtbHMzZzRoYmYxcDZlaHFjcjI3ajhhdDRidnNmMWdwZDRvMCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/4pMX5rJ4PYAEM/giphy.gif",

                    };
                    var ListaGifs25 = new List<string>
                    {
                    "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExdGltMnVqYXp6MzlpanFnem80Z2k3dTJ4ZDRzNXRzZjg0cGN3ZWlnNyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/MF0QiCa9JPEI6HiaCK/giphy.gif",
                    "https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExd2tyZ2FnZTdraWRnY3B3YXcxandyeHkzcTVqa2JnNDNpYWM4eHp5ayZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/yr7n0u3qzO9nG/giphy.gif",
                    "https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExZWk4MzVtOWYxY2l2NzB2aGVtZXlkMnJzN3QwZHB5dXkxbXl5MmJkMiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/UKF08uKqWch0Y/giphy.gif,",
                    };


                    string gif = string.Empty;
                    string mensagem = string.Empty;
                    if (dados.Engajamento > 100)
                    {
                        gif = ListasGifs(ListaGifs50);
                        mensagem = $@"Seu engajamento foi de <b>{dados.Engajamento}% e lançamentos pelo Devops de {string.Format("{0:#.#,##}", Math.Round(dados.DevOps, 1))}%. Você registrou um tempo de trabalho superior em relação ao ponto.</b> Atenção com os lançamentos.";
                    }
                    else if (dados.Engajamento >= 90 && dados.DevOps >= 95)
                    {
                        mensagem = $"Seus lançamentos estão excelentes, <b>parabéns</b>✅. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{string.Format("{0:#.#,##}", Math.Round(dados.DevOps, 1))}%</b>.";
                        gif = ListasGifs(ListaGifs90);
                    }
                    else if (dados.Engajamento >= 0 && dados.DevOps >= 95)
                    {
                        mensagem = $"<b>Atenção com os lançamentos no dia à dia</b>⚠️. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{string.Format("{0:#.#,##}", Math.Round(dados.DevOps, 1))}%</b>.";
                        gif = ListasGifs(ListaGifs50);
                    }
                    else if (dados.Engajamento >= 90 && dados.DevOps < 95)
                    {
                        mensagem = $"<b>Atenção com os lançamentos dentro das atividades do Devops</b>⚠️. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{string.Format("{0:#.#,##}", Math.Round(dados.DevOps, 1))}%</b>.";
                        gif = ListasGifs(ListaGifs50);
                    }
                    else if (dados.Engajamento < 90 && dados.DevOps < 95)
                    {
                        mensagem = $"<b>Seus lançamentos não estão legais</b>, mais atenção com os lançamentos do dia à dia e dentro das atividades do Devops ❌. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{string.Format("{0:#.#,##}", Math.Round(dados.DevOps, 1))}%</b>.";
                        gif = ListasGifs(ListaGifs25);

                    }

                    // Configuração do corpo do e-mail
                    mailItem.Subject = $@"[SISTEMAS] APURAÇÃO DE HORAS SEMANAL - {pessoa.Nome.ToUpper()} ";
                    mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                    string msgHTMLBody = $@"
                     <html>
                        <head></head>
                        <body>
                            Boa Tarde, {pessoa.Nome},<br><br>
                            Segue o dashboard com os lançamentos do período.<b>{dados.Periodo}.</b><br><br>
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

                    // Envio do e-mail
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

        private int CalcularEngajamento(IEnumerable<TMetric> tmetrics, IEnumerable<Ponto> pontos, Colaborador colaborador)
        {
            var hora = GetHoras(tmetrics.Where(c => c.ColaboradorId.ToString().ToUpper() == colaborador.Id.ToString().ToUpper()).Select(h => h.Duracao));
            var ponto = GetHoras(pontos.Where(p => p.ColaboradorId.ToString().ToUpper() == colaborador.Id.ToString().ToUpper()).Select(h => h.Horas));
            if (ponto == 0)
            {
                ponto = colaborador.CargaHoraria;
                if (ponto < hora)
                    ponto = hora;
            }

            return (int)Math.Round(hora / ponto * 100, 0);

        }
        private string ListasGifs(List<string> gifs)
        {
            var random = new Random();
            int randomIndex = random.Next(0, gifs.Count);
            return gifs[randomIndex];
        }

        // Função privada para calcular o total de horas a partir de uma coleção de datas
        private double GetHoras(IEnumerable<DateTime> dates)
        {
            // Inicializa o total de horas como zero
            var total = new TimeSpan();

            // Itera sobre as datas e soma as horas
            foreach (var item in dates.Select(c => c.TimeOfDay))
                total = total.Add(item);

            // Retorna o total de horas em formato de horas
            return total.TotalHours;
        }

        //[Route("ListarFuncionarios")]
        //[Route("ListarFuncionarios")]
        //[HttpGet]
        //public async Task<IActionResult> ListarFuncionario()
        //{
        //    using (var db = new BISistemasContext())
        //    {
        /*
        var dados = db.Colaboradores;
        var TMS = dados.Where(x => x.Time == "TMS").ToList();
        var ERP = dados.Where(x => x.Time == "ERP");

        var simples = TMS.Select(s => new
        {
            Nome = s.Nome,
            Cargo = s.Cargo
        }).ToList();

        var dados = db.TMetrics;

        var amanda = dados.Where(x => x.Usuario == "Amanda Ferreira" && x.DevopsTask == 15722).ToList();
        */

        //        var dados = db.TMetrics;
        //        var amanda = dados.Where(x => x.Usuario == "Amanda Ferreira").ToList();


        //        var dados1 = db.Colaboradores;
        //        var amanda1 = dados1.Where(x => x.UserTMetric == "Amanda Ferreira").ToList();

        //        var conjunto = new
        //        {
        //            TMS = amanda.Select(x => x.Usuario),
        //            Colabi = amanda1.Select(x => x.Nome),
        //        };



        //    }



        //    return null;
        //}

    }
}
