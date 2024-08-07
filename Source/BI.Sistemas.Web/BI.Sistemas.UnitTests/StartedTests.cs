using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace BI.Sistemas.UnitTests
{
    [TestClass]
    public class StartedTests
    {
        [TestMethod]
        public void CriarColaboradores()
        {
            using (var db = new BISistemasContext())
            {
                var colaboradores = db.Colaboradores.AsQueryable().ToList();

                AddColaborador(db, colaboradores, "Fernanda Cassiano", "Analista de Neg�cios", "ERP", "Fernanda Cassiano Pereira dos Santos", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-fernanda.jpg");
                AddColaborador(db, colaboradores, "Paulo Silva", "Analista de Neg�cios", "TMS", "Paulo Rafael da Silva", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-paulo.jpg");
                AddColaborador(db, colaboradores, "Amanda Ferreira", "Analista de Neg�cios", "ERP", "Amanda Ferreira", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-amanda.jpg");
                AddColaborador(db, colaboradores, "Luiz Oliveira", "Analista de Sistemas", "ERP", "Luiz Oliveira (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-luiz.jpg");
                AddColaborador(db, colaboradores, "Marco Tulio Rodrigues", "Coordenador de Sistemas", "TMS", "Marco Tulio Rodrigues", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-tulio.png");
                AddColaborador(db, colaboradores, "Thiago Oliveira", "Analista de Sistemas", "ERP", "Thiago Gomes", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-thiago.jpg");
                AddColaborador(db, colaboradores, "Jo�o Paulo", "Analista de Neg�cios", "TMS", "Joao Paulo (JP TI)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-jp.jpg");
                AddColaborador(db, colaboradores, "Rog�rio Sim�es de Oliveira", "Especialista de Sistemas", "Sistemas", "Rog�rio Sim�es de Oliveira", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-rogerio.jpg");
                AddColaborador(db, colaboradores, "Junior Dias", "Analista de Sistemas", "TMS", "Junior Dias", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-junior.jpg");
                AddColaborador(db, colaboradores, "Joel Junior", "Analista de Neg�cios", "TMS", "Joel Martins J�nior", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-joel.jpg");
                AddColaborador(db, colaboradores, "Marco Aurelio Barros", "Coordenador de Sistemas", "ERP", "Marco Aurelio (COORD TI ERP)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-marco.jpg");
                AddColaborador(db, colaboradores, "Petronio Faleixo", "Analista de Sistemas", "TMS", "Petronio Faleixo", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-petronio.jpg");


                AddColaborador(db, colaboradores, "Giovanni de Souza Campos", "Analista de Sistemas SR", "Suporte ERP", "Giovanni Campos (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-giovanni.jpg");
                AddColaborador(db, colaboradores, "Mateus de Oliveira Menezes Aquino", "Analista de Suporte JR", "Suporte TMS", "Mateus Menezes (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-mateus.jpg");
                AddColaborador(db, colaboradores, "Isaias Oliveira Guimaraes", "Analista de Suporte PL", "Suporte TMS", "Isaias Oliveira (TI MTZ)   ", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-isaias.jpg");
                AddColaborador(db, colaboradores, "Vitor dos Santos Gomes", "Analista de Suporte JR", "Suporte TMS", "Vitor Gomes (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-vitinho.jpg");
                AddColaborador(db, colaboradores, "Euller Neviton Vieira", "Analista EDI PL", "EDI", "Euller Vieira (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-euller.jpg");
                AddColaborador(db, colaboradores, "Paulo Junior Souza Ramos", "Analista de Suporte PL", "Suporte TMS", "Paulo Souza  (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-paulinho.jpg");
                AddColaborador(db, colaboradores, "Anna Paula Gomes da Silva", "Assistente de Sistemas", "Suporte TMS", "Anna Silva (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-anna.jpg");
                AddColaborador(db, colaboradores, "Joao Pedro Martins dos Santos", "Assistente de Sistemas", "Suporte TMS", "Jo�o Santos (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-joao-santos.jpg");
                AddColaborador(db, colaboradores, "Ricardo Rodrigues dos Santos", "Analista de Sistemas SR", "CRM", "Ricardo Santos (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-ricardo.jpg");
                AddColaborador(db, colaboradores, "Samuel Rodrigo Lopes Ferreira", "Auxiliar Help Desk", "Suporte ERP", "Samuel Rodrigo Ferreira (Auxiliar TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-samuel.jpg");
                AddColaborador(db, colaboradores, "Barbara Barros", "Estagi�rio CRM", "CRM", "Barbara Barros (EST-TECH TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-barbara.jpg");
                AddColaborador(db, colaboradores, "Arthur Abreu", "Estagi�rio ERP", "Suporte ERP", "Arthur Abreu (EST-TECH TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-arthur.jpg");
                AddColaborador(db, colaboradores, "Vitor Fernandes", "Estagi�rio TMS", "TMS", "Vitor Souza (EST-TECH TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-vitor.jpg");
                AddColaborador(db, colaboradores, "Kleverson Salles", "Analista de Sistemas SR", "Suporte ERP", "Kleverson Salles (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-kleverson.jpg");
                AddColaborador(db, colaboradores, "Natalia Caroline", "Supervisor de EDI", "EDI", "Natalia Caroline (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-natalia.jpg");
                AddColaborador(db, colaboradores, "Marcus Vin�cius", "Supervisor de Suporte", "Suporte TMS", "Marcus Vin�cius (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-marcus.jpg");
                AddColaborador(db, colaboradores, "Marcus Ethur", "Supervisor de Suporte", "Suporte ERP", "Marcus Ethur (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-ethur.jpg");
                AddColaborador(db, colaboradores, "Joselito Almeida", "Analista de Sistemas SR", "Suporte ERP", "Joselito Almeida (TI MTZ)", @"C:\Users\vitor.fernandessouza\Desktop\DashBoard\UI\Content\Images\face-joselito.jpg");

                db.SaveChanges();
            }
        }

        private void AddColaborador(BISistemasContext db,
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

                db.Colaboradores.Add(colaborador);
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
            File.ReadAllLines(@"C:\Users\vitor.fernandessouza\Downloads\detailed_report_20240415_20240421 (5).csv")
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

        [TestMethod]
        public void CargaHE()
        {
            CargaPonto(TipoPonto.HE);
        }
        [TestMethod]
        public void CargaPontoNormal()
        {
            CargaPonto(TipoPonto.Normal);
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
                File.ReadAllLines(@"C:\Users\vitor.fernandessouza\Downloads\PontoCLT.csv")
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

                //foreach (var colaborador in db.Colaboradores)
                //    {
                //        db.Pontos.Add(new Ponto()
                //        {
                //            Colaborador = colaborador,
                //            PeriodoId = periodo.Id,
                //            Periodo = periodo,
                //            Horas = periodo.Data.AddHours(Random.Shared.Next(10)),
                //            Tipo = tipo,
                //        });
                //    }

                //    db.SaveChanges();
            }
        }

        [TestMethod]
        public void CargaMovidesk()
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
            File.ReadAllLines(@"C:\Users\vitor.fernandessouza\Downloads\Movidesk - Base chamados TI - Semana Retroativa_novo.csv")
                .Skip(1)
                .ToList()
                .ForEach(v =>
                {
                    using (var db = new BISistemasContext())
                    {
                        var periodo = db.Periodos.FirstOrDefault(c => c.Data == DateTime.Today);
                        var movidesk = Movidesk.FromCsv(v);
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
                var geracao = new BI.Sistemas.Domain.Novo.Geracao();
                geracao.Data = DateTime.Today.AddDays(-1);
                geracao.DataFim = DateTime.Today.AddDays(3);
                geracao.DataInicio = DateTime.Today.AddDays(23);

                geracao.Times.Add(new Domain.Novo.GeracaoTime()
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

                geracao.Times.Add(new Domain.Novo.GeracaoTime()
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

                geracao.Times.Add(new Domain.Novo.GeracaoTime()
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
    }

}
