using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Outlook = Microsoft.Office.Interop.Outlook;
using static BI.Sistemas.API.View.ColaboradorDashboardView;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

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

                var pontosTodos = db.Pontos.ToList();
                var tmetricTodos = db.TMetrics.Where(a =>
                a.Usuario != "Petronio Faleixo" &&
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
                colaborador.FotoTime = Convert.ToBase64String(System.IO.File.ReadAllBytes($@"{projectDirectory}\UI\Content\Images\time-{pessoa.Time}.png"));
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

                // Calculando o engajamento de cada pessoa e guardando em um dicionário
                var excludes = new string[]
                {
                    "010A52D3-D812-4DCE-D721-08DC62DFD032",
                    "D2E3452C-E900-4573-D724-08DC62DFD032",
                    "22654802-4100-4CF7-D727-08DC62DFD032"
                };

                var engajamentos = new Dictionary<Colaborador, int>();
                foreach (var col in db.Colaboradores.ToList().Where(c => !excludes.Contains(c.Id.ToString().ToUpper())).ToList())
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
                    //colaborador.HE_Equipe = "-04:29";


                }
                else if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                {
                    colaborador.HE_Individual = "01:19";
                    colaborador.HE_Equipe = "01:19";

                }
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                {
                    colaborador.HE_Individual = "44:00";
                    //colaborador.HE_Equipe = "03:05";

                }
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                {
                    colaborador.HE_Individual = "04:17";
                    colaborador.HE_Equipe = "02:35";

                }
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                {
                    colaborador.HE_Individual = "-02:21";
                    colaborador.HE_Equipe = "02:35";

                }
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                {
                    colaborador.HE_Individual = "44:00";
                    //colaborador.HE_Equipe = "-04:29";

                }
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                {
                    colaborador.HE_Individual = "-03:51";
                    colaborador.HE_Equipe = "02:35";

                }
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                {
                    colaborador.HE_Individual = "00:00";
                    colaborador.HE_Equipe = "01:19";

                }

                var anterior1 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior1 = 95;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior1 = 98;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior1 = 72;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior1 = 84;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior1 = 99;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior1 = 98;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior1 = 86;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior1 = 99;

                var anterior2 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior2 = 95;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior2 = 100;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior2 = 54;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior2 = 94;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior2 = 91;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior2 = 23;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior2 = 91;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior2 = 90;

                var anterior3 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior3 = 98;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") //Fernanda Cassiano ERP
                    anterior3 = 98;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior3 = 81;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior3 = 96;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior3 = 96;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior3 = 99;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior3 = 96;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior3 = 99;

                var anterior4 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior4 = 97;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior4 = 88;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior4 = 100;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior4 = 91;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior4 = 101;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior4 = 100;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior4 = 102;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior4 = 97;


                var anterior5 = 0;

                if (id == "69DB13EF-89C0-4A6F-D71F-08DC62DFD032") // Amanda Ferreira ERP
                    anterior5 = 97;
                else if (id == "BD984996-9C11-4095-D71D-08DC62DFD032") // Fernanda Cassiano ERP
                    anterior5 = 71;
                else if (id == "52F14677-9C85-41D5-D723-08DC62DFD032") // João Paulo TMS
                    anterior5 = 81;
                else if (id == "C44D7319-3318-43D4-D726-08DC62DFD032") // Joel Junior TMS
                    anterior5 = 93;
                else if (id == "3F7E1A71-815A-4397-D725-08DC62DFD032") // Junior Dias TMS
                    anterior5 = 102;
                else if (id == "0D6227A1-7B72-4DAC-D720-08DC62DFD032") // Luiz Oliveira ERP
                    anterior5 = 80;
                else if (id == "C0D4394F-38EF-4F8B-D71E-08DC62DFD032") // Paulo Silva TMS
                    anterior5 = 91;
                else if (id == "87B833CD-7810-4030-D722-08DC62DFD032") // Thiago Oliveira ERP
                    anterior5 = 99;


                var lista = new List<EvolucaoEngajamentoView>();


                //lista.Add(new EvolucaoEngajamentoView() { Data = "03/05", Valor = anterior1 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "10/05", Valor = anterior2 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "17/05", Valor = anterior3 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "24/05", Valor = anterior4 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "31/05", Valor = anterior5 });
                lista.Add(new EvolucaoEngajamentoView() { Data = "03/06", Valor = colaborador.Engajamento });

                colaborador.EvolucaoEngajamento = lista.ToArray();
                // Retornando as informações do painel do colaborador
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
                    string mensagem;
                    if (dados.Engajamento > 100)
                    {
                        gif = ListasGifs(ListaGifs50);
                        mensagem = $@"Seu engajamento foi de <b>{dados.Engajamento}% Você registrou um tempo de trabalho superior em relação ao ponto.</b> Atenção com os lançamentos.";
                    }
                    else if (dados.Engajamento >= 90)
                    {
                        mensagem = $"Seu engajamento foi de <b>{dados.Engajamento}%</b>, seu lançamento está excelente, <b>parabéns</b>✅";
                        gif = ListasGifs(ListaGifs90);
                    }
                    else if (dados.Engajamento >= 50)
                    {
                        mensagem = $"Seu engajamento foi de <b>{dados.Engajamento}%</b>, essa semana seu lançamento ficou <b>um pouco abaixo</b>⚠️.";
                        gif = ListasGifs(ListaGifs50);
                    }
                    else
                    {
                        gif = ListasGifs(ListaGifs25);
                        mensagem = $@"Seu engajamento foi de <b>{dados.Engajamento}%</b>, essa semana não foi legal.<b><span style=""color:red;""> Atenção com os lançamentos ❌.</b></span>";
                    }
                    
                    // Configuração do corpo do e-mail
                    mailItem.Subject = $@"[SISTEMAS] APURAÇÃO DE HORAS SEMANAL - {pessoa.Nome.ToUpper()} ";
                    mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                    string msgHTMLBody = $@"
                     <html>
                        <head></head>
                        <body>
                            Boa Tarde, {pessoa.Nome},<br><br>
                            Segue o dashboard com os lançamentos do período.<b><br>{dados.Periodo}.</b><br><br>
                            <div style=""min-width:600px!important;margin-left:auto;margin-right:auto; ""></div>
                            <img src=""{gif}"" width=""80"" height=""80""/><br>
                            {mensagem}<br><br>
                            <img align=""baseline"" border=""1"" hspace=""0"" src=""{dados.Foto}"" width=""880"" 600="""" hold="" /> ""></img><br>
                            <i>*** O novo percentual ({string.Format("{0:#.#,##}", Math.Round(dados.DevOps,1))}%) no dashboard é referente aos lançamentos de Discovery, Delivery e Bug com o card do Devops referenciado. O ideal é sempre iniciar uma tarefa destes tipos de apropriação diretamente do Azure Devops.</i>
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
