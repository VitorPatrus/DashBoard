using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Entities.Enums;
using BI.Sistemas.Domain.Novo;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
// Projeto Correto!

namespace BI.Sistemas.UnitTests
{
    [TestClass]
    public class StartedTests
    {
        BISistemasContext _dbcontext;
        string _baseDirectory;

        [TestInitialize]
        public void Init()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo projectDirectory = null;
            if (_baseDirectory != null)
                projectDirectory = Directory.GetParent(_baseDirectory).Parent.Parent.Parent.Parent.Parent.Parent;

            _dbcontext = new BISistemasContext();
        }

        [TestMethod]
        public void CriarColaboradores()
        {
            var colaboradores = _dbcontext.Colaboradores.AsQueryable().ToList();

            //AddColaborador(colaboradores, "Fernanda Cassiano", "Analista de Negócios", "ERP", "Fernanda Cassiano Pereira dos Santos", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-fernanda.jpg");
            //AddColaborador(db, colaboradores, "Paulo Silva", "Analista de Negócios", "TMS", "Paulo Rafael da Silva", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-paulo.jpg");
            //AddColaborador(db, colaboradores, "Amanda Ferreira", "Analista de Negócios", "ERP", "Amanda Ferreira", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-amanda.jpg");
            //AddColaborador(db, colaboradores, "Luiz Oliveira", "Analista de Sistemas", "ERP", "Luiz Oliveira (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-luiz.jpg");
            //AddColaborador(db, colaboradores, "Marco Tulio Rodrigues", "Coordenador de Sistemas", "TMS", "Marco Tulio Rodrigues", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-tulio.png");
            //AddColaborador(db, colaboradores, "Thiago Oliveira", "Analista de Sistemas", "ERP", "Thiago Gomes", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-thiago.jpg");
            //AddColaborador(db, colaboradores, "João Paulo", "Analista de Negócios", "TMS", "Joao Paulo (JP TI)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-jp.jpg");
            //AddColaborador(db, colaboradores, "Rogério Simões de Oliveira", "Especialista de Sistemas", "Sistemas", "Rogério Simões de Oliveira", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-rogerio.jpg");
            //AddColaborador(db, colaboradores, "Junior Dias", "Analista de Sistemas", "TMS", "Junior Dias", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-junior.jpg");
            //AddColaborador(db, colaboradores, "Joel Junior", "Analista de Negócios", "TMS", "Joel Martins Júnior", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-joel.jpg");
            //AddColaborador(db, colaboradores, "Marco Aurelio Barros", "Coordenador de Sistemas", "ERP", "Marco Aurelio (COORD TI ERP)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-marco.jpg");
            //AddColaborador(db, colaboradores, "Petronio Faleixo", "Analista de Sistemas", "TMS", "Petronio Faleixo", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-petronio.jpg");
            //AddColaborador(db, colaboradores, "Giovanni de Souza Campos", "Analista de Sistemas SR", "Suporte ERP", "Giovanni Campos (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-giovanni.jpg");
            //AddColaborador(db, colaboradores, "Mateus de Oliveira Menezes Aquino", "Analista de Suporte JR", "Suporte TMS", "Mateus Menezes (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-mateus.jpg");
            //AddColaborador(db, colaboradores, "Isaias Oliveira Guimaraes", "Analista de Suporte PL", "Suporte TMS", "Isaias Oliveira (TI MTZ)   ", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-isaias.jpg");
            //AddColaborador(db, colaboradores, "Vitor dos Santos Gomes", "Analista de Suporte JR", "Suporte TMS", "Vitor Gomes (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-vitinho.jpg");
            //AddColaborador(db, colaboradores, "Euller Neviton Vieira", "Analista EDI PL", "EDI", "Euller Vieira (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-euller.jpg");
            //AddColaborador(db, colaboradores, "Paulo Junior Souza Ramos", "Analista de Suporte PL", "Suporte TMS", "Paulo Souza  (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-paulinho.jpg");
            //AddColaborador(db, colaboradores, "Anna Paula Gomes da Silva", "Assistente de Sistemas", "Suporte TMS", "Anna Silva (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-anna.jpg");
            //AddColaborador(db, colaboradores, "Joao Pedro Martins dos Santos", "Assistente de Sistemas", "Suporte TMS", "João Santos (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-joao-santos.jpg");
            //AddColaborador(db, colaboradores, "Ricardo Rodrigues dos Santos", "Analista de Sistemas SR", "CRM", "Ricardo Santos (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-ricardo.jpg");
            //AddColaborador(db, colaboradores, "Samuel Rodrigo Lopes Ferreira", "Auxiliar Help Desk", "Suporte ERP", "Samuel Rodrigo Ferreira (Auxiliar TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-samuel.jpg");
            //AddColaborador(db, colaboradores, "Barbara Barros", "Estagiário CRM", "CRM", "Barbara Barros (EST-TECH TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-barbara.jpg");
            //AddColaborador(db, colaboradores, "Arthur Abreu", "Estagiário ERP", "Suporte ERP", "Arthur Abreu (EST-TECH TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-arthur.jpg");
            //AddColaborador(db, colaboradores, "Vitor Fernandes", "Estagiário TMS", "TMS", "Vitor Souza (EST-TECH TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-vitor.jpg");
            //AddColaborador(db, colaboradores, "Kleverson Salles", "Analista de Sistemas SR", "Suporte ERP", "Kleverson Salles (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-kleverson.jpg");
            //AddColaborador(db, colaboradores, "Natalia Caroline", "Supervisor de EDI", "EDI", "Natalia Caroline (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-natalia.jpg");
            //AddColaborador(db, colaboradores, "Marcus Vinícius", "Supervisor de Suporte", "Suporte TMS", "Marcus Vinícius (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-marcus.jpg");
            //AddColaborador(db, colaboradores, "Marcus Ethur", "Supervisor de Suporte", "Suporte ERP", "Marcus Ethur (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-ethur.jpg");
            //AddColaborador(db, colaboradores, "Joselito Almeida", "Analista de Sistemas SR", "Suporte ERP", "Joselito Almeida (TI MTZ)", @"C:\Users\vitor.fernandessouza\Documents\DashBoard\UI\Content\Images\face-joselito.jpg");

            _dbcontext.SaveChanges();
        }

        [TestMethod]
        public void FazerTudo()
        {
            InserirBancoDeDados();
            CargaTMetric();
            CargaPonto(TipoPonto.Normal);
        }

        private void AddColaborador(
           IEnumerable<Colaborador> colaboradores,
           string nome, string cargo, string time,
           string userTMetric,
           string? fileNameFoto = null)
        {
            if (!colaboradores.Any(c => c.Nome == nome))
            {
                var colaborador = new Colaborador()
                {
                    Nome = nome,
                    Cargo = cargo,
                    CargaHoraria = 44,
                    Inicio = new DateTime(2024, 01, 01, 08, 00, 00),
                    Termino = new DateTime(2024, 01, 01, 18, 00, 00),
                    Time = time,
                    UserTMetric = userTMetric

                };

                if (File.Exists(fileNameFoto))
                    colaborador.Foto = File.ReadAllBytes(fileNameFoto);

                _dbcontext.Colaboradores.Add(colaborador);
            }
        }

        [TestMethod]
        public void CargaTMetric()
        {
            using (var db = new BISistemasContext())
            {
                var periodo = db.Periodos.FirstOrDefault(c => c.Data == DateTime.Today);
                if (periodo == null)
                {
                    periodo = new Periodo();
                    periodo.Data = DateTime.Today;
                    periodo.Inicio = new DateTime(2024, 4, 15);
                    periodo.Termino = new DateTime(2024, 4, 19);
                    db.Periodos.Add(periodo);
                    db.SaveChanges();
                }
            }
            var dataCarga = DateTime.Now;
            File.ReadAllLines($@"{_baseDirectory}\UI\Excel\TMETRIC.csv")
                .Skip(1)
                .ToList()
                .ForEach(v =>
                {
                    using (var db = new BISistemasContext())
                    {
                        var periodo = db.Periodos.FirstOrDefault(c => c.Data == DateTime.Today);
                        var metric = TMetric.FromCsv(dataCarga, v);
                        var colaborador = db.Colaboradores.FirstOrDefault(c => c.UserTMetric.ToUpper() == metric.Usuario.ToUpper());
                        metric.Colaborador = colaborador;
                        metric.Periodo = periodo;
                        db.TMetrics.Add(metric);
                        db.SaveChanges();
                    }
                });
        }

        private static void CargaPonto(TipoPonto tipo)
        {
            using (var db = new BISistemasContext())
            {
                var periodo = db.Set<Periodo>().FirstOrDefault(c => c.Data == DateTime.Today);
                if (periodo == null)
                {
                    periodo = new Periodo();
                    periodo.Data = DateTime.Today;
                    periodo.Inicio = new DateTime(2024, 4, 15);
                    periodo.Termino = new DateTime(2024, 4, 19);
                    db.Periodos.Add(periodo);

                }

                File.ReadAllLines($@"C:\Users\petronio.aleixo\Desktop\DashBoard\UI\Excel\PontoCLT.csv")
                    .Skip(1)
                .ToList()
                .ForEach(v =>
                {
                    using (var db = new BISistemasContext())
                    {
                        var periodo = db.Periodos.FirstOrDefault(c => c.Data == DateTime.Today);
                        var colaboradores = db.Colaboradores.ToList();
                        var ponto = Ponto.FromCsv(colaboradores, v);
                        ponto.Periodo = periodo;
                        db.Pontos.Add(ponto);
                        db.SaveChanges();
                    }
                });
            }
        }

        // SLA
        [TestMethod]
        public void CargaMovidesk()
        {
            //var data1 = new DateTime(2024,09,12); 
            var data1 = DateTime.Today;
            using (var db = new BISistemasContext())
            {
                var periodo = db.Periodos.FirstOrDefault(c => c.Data == data1);
                if (periodo == null)
                {
                    periodo = new Periodo();
                    periodo.Data = data1;
                    periodo.Inicio = new DateTime(2024, 4, 15);
                    periodo.Termino = new DateTime(2024, 4, 19);
                    _dbcontext.Periodos.Add(periodo);
                    _dbcontext.SaveChanges();
                }
            }

            File.ReadAllLines($@"{_baseDirectory}\UI\Excel\RelatorioTI_SolicitaçõesAbertas (3).csv")
              .Skip(1)
              .Where(x => !string.IsNullOrWhiteSpace(x))
              .ToList()
              .ForEach(v =>
              {
                  using (var db = new BISistemasContext())
                  {
                      var periodo = db.Periodos.FirstOrDefault(c => c.Data == data1);
                      var movidesk = Movidesk.FromCsv(v);

                      if (movidesk.Time != "STI" && db.Movidesks.Any(c => c.Numero == movidesk.Numero && c.PeriodoId.ToString().ToUpper() == periodo.ToString().ToUpper()))
                          return;
                      movidesk.Periodo = periodo;

                      var colaborador = db.Colaboradores.FirstOrDefault(c => c.UserTMetric
                      .ToUpper() == movidesk.ResponsavelChamado.ToUpper());
                      movidesk.Responsavel = colaborador;
                      movidesk.Periodo = periodo;

                      db.Movidesks.Add(movidesk);
                      db.SaveChanges();
                  }
              });

            File.ReadAllLines($@"{_baseDirectory}\UI\Excel\RelatorioTI_SolicitaçõesSemanaAnterior (3).csv")
                .Skip(1)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList()
                .ForEach(v =>
                {
                    using (var db = new BISistemasContext())
                    {
                        var periodo = db.Periodos.FirstOrDefault(c => c.Data == data1);

                        var movidesk = Movidesk.FromCsv(v);

                        if (movidesk.Time != "STI" && db.Movidesks.Any(c => c.Numero == movidesk.Numero && c.PeriodoId.ToString().ToUpper() == periodo.ToString().ToUpper()))
                            return;
                        movidesk.Periodo = periodo;

                        var colaborador = db.Colaboradores.FirstOrDefault(c => c.UserTMetric
                        .ToUpper() == movidesk.ResponsavelChamado.ToUpper());
                        movidesk.Responsavel = colaborador;
                        movidesk.Periodo = periodo;

                        db.Movidesks.Add(movidesk);
                        db.SaveChanges();
                    }
                });
        }

        [TestMethod]
        public void SomaHora()
        {
            using (var db = new BISistemasContext())
            {
                var lancamentos = db.TMetrics.AsQueryable().ToList();
                var total = new TimeSpan();
                foreach (var item in lancamentos.Select(c => c.Duracao.TimeOfDay))
                    total = total.Add(item);

                var totalHours = total.TotalHours;
                var perc = totalHours / 44;
            }
        }

        [TestMethod]
        public void CriarGeracaoNovo()
        {
            using (var db = new BISistemasContext())
            {
                var data = DateTime.Now;
                var geracao = new Geracao();
                geracao.Data = data.AddDays(-1);
                geracao.DataFim = data.AddDays(3);
                geracao.DataInicio = data.AddDays(23);

                geracao.Times.Add(new GeracaoTime
                {
                    ConclusaoCards = 10,
                    SpEntregues = 7,
                    BacklogTotal = 0,
                    BacklogNovo = 30,
                    Wip = 21,
                    Bug = 22,
                    LeadTime = 23,
                    CicleTime = 24,
                });

                geracao.Times.Add(new GeracaoTime
                {
                    ConclusaoCards = 30,
                    SpEntregues = 47,
                    BacklogTotal = 20,
                    BacklogNovo = 50,
                    Wip = 11,
                    Bug = 72,
                    LeadTime = 73,
                    CicleTime = 4,

                });

                geracao.Times.Add(new GeracaoTime
                {
                    ConclusaoCards = 1,
                    SpEntregues = 5,
                    BacklogTotal = 30,
                    BacklogNovo = 32,
                    Wip = 12,
                    Bug = 25,
                    LeadTime = 32,
                    CicleTime = 45,

                });
                db.Add(geracao);
                db.SaveChanges();
            }
        }

        [TestMethod]
        private void InserirBancoDeDados()
        {
            string csvFilePath = $@"{_baseDirectory}\UI\Excel\Downloads\HE.csv";

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                csv.Context.RegisterClassMap<ColaboradorRecordMap>();
                var records = csv.GetRecords<ColaboradorRecord>();

                using (var db = new BISistemasContext())
                {
                    foreach (var record in records)
                    {
                        var colaborador = db.Colaboradores.FirstOrDefault(c => c.Matricula.ToString() == record.Colaborador);
                        if (colaborador != null)
                        {
                            decimal horas = ConverterParaDecimalHoras(record.Banco.ToString());

                            var he = new HE
                            {
                                ColaboradorId = colaborador.Id,
                                Horas = horas,
                                Data = DateTime.Today,
                                PeriodoId = ObterOuCriarPeriodo()
                            };
                            db.HorasExtras.Add(he);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        static decimal ConverterParaDecimalHoras(string planilha)
        {
            bool ehNegativo = planilha.Contains("-");
            planilha = planilha.Replace("-", "").Trim();

            string[] horaMinuto = planilha.Split(':');

            if (horaMinuto.Length != 3)
            {
                throw new FormatException("O formato deve ser hh:mm:ss");
            }

            if (!decimal.TryParse(horaMinuto[0], CultureInfo.InvariantCulture, out decimal horas) ||
                horas < 0 || horas > 99.99m)
            {
                throw new FormatException("O valor de horas não está no formato esperado. A parte das horas deve ser numérica e ter no máximo duas casas decimais.");
            }

            if (!int.TryParse(horaMinuto[1], out int minutos) ||
                !int.TryParse(horaMinuto[2], out int segundos))
            {
                throw new FormatException("O valor de horas não está no formato correto");
            }

            if (minutos < 0 || minutos >= 60 || segundos < 0 || segundos >= 60)
            {
                throw new FormatException("Os minutos e segundos, maiores que 60 ou menores que 0.");
            }

            decimal totalHoras = horas + (minutos / 60m) + (segundos / 3600m);
            return ehNegativo ? -totalHoras : totalHoras;
        }

        private static Guid ObterOuCriarPeriodo()
        {
            using (var db = new BISistemasContext())
            {
                var periodo = db.Periodos.FirstOrDefault(c => c.Data == DateTime.Today);
                if (periodo == null)
                {
                    periodo = new Periodo
                    {
                        Data = DateTime.Today,
                        Inicio = new DateTime(2024, 4, 15),
                        Termino = new DateTime(2024, 4, 19)
                    };
                    db.Periodos.Add(periodo);
                    db.SaveChanges();
                }
                return periodo.Id;
            }
        }
    }
}