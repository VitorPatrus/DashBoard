using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain.Entities.Enums;
using BI.Sistemas.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static BI.Sistemas.API.View.ColaboradorDashboardView;
using BI.Sistemas.Domain.Extensions;
using System.Runtime.CompilerServices;
using BI.Sistemas.API.Interfaces;
using BI.Sistemas.API.Repository;

namespace BI.Sistemas.API.Service
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly IColaboradorRepository _colaboradorRepository;
        public ColaboradorService(IColaboradorRepository colaboradorRepository)
        {
            _colaboradorRepository = colaboradorRepository;
        }
        public  ColaboradorDashboardView GetColaboradorDashboard(string id)
        {
            TMetric metricas = new TMetric();
            if (id.IsNullOrEmpty())
                throw new Exception("O ID do colaborador não foi fornecido!");
            
                var pontosTodos = _colaboradorRepository.GetPonto();
                var tmetricTodos =  _colaboradorRepository.GetTodos();
                var pessoa = _colaboradorRepository.GetPessoa(id);

                if (pessoa == null)
                    throw new Exception($"Colaborador não encontrado (ID: {id})");

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
                var periodoAtual = _colaboradorRepository.GetPeriodo();

                colaborador.FotoTime = Convert.ToBase64String(System.IO.File
                   .ReadAllBytes($@"{projectDirectory}\UI\Content\Images\time-{pessoa.Time}.png"));

                var HE = pontosTodos
                .Where(he => he.PeriodoId.EqualsGuid(periodoAtual.Id))
                .LastOrDefault();

                var listaColaboradores = _colaboradorRepository.GetListaColaboradores();

                var HoraPonto = pontosTodos
                    .Where(he => he.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper() && he.ColaboradorId
                    .ToString().Equals(id, StringComparison.OrdinalIgnoreCase) && he.Tipo == TipoPonto.Normal)
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
                    .Where(tm => tm.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString()
                    .ToUpper() && tm.ColaboradorId.ToString().ToUpper() == id)
                    .ToList();

                var metrics = tmetricTodos.Where(m => m.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString().ToUpper()).ToList();

                var pontos = pontosTodos
                    .Where(x => x.PeriodoId.ToString().ToUpper() == HE.PeriodoId.ToString()
                    .ToUpper() && x.Tipo == TipoPonto.Normal).ToList();

                var excludes = new string[]
                {
                    "010A52D3-D812-4DCE-D721-08DC62DFD032",
                    "D2E3452C-E900-4573-D724-08DC62DFD032",
                    "22654802-4100-4CF7-D727-08DC62DFD032",
                    "7B71C413-0E95-409B-D7DB-08DCC077B61A"
                };

                var engajamentos = new Dictionary<Colaborador, int>();
                foreach (var col in listaColaboradores
                    .Where(c => c.Time == "TMS" || c.Time == "ERP")
                    .Where(c => !excludes.Contains(c.Id.ToString().ToUpper()))
                    .Where(c => c.CargaHoraria > 0 || pontos.Exists(x => x.ColaboradorId?
                    .ToString().ToUpper() == c.Id.ToString().ToUpper()))
                    .ToList())
                    engajamentos.Add(col, metricas.CalcularEngajamento(metrics, pontos, col));

                var engajamentoTime = engajamentos.Where(e => e.Key.Time == pessoa.Time).ToList();

                colaborador.Engajamento = engajamentos.FirstOrDefault(e => e.Key.Id.ToString().ToUpper() == pessoa.Id
                .ToString().ToUpper()).Value;
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

                var peneira = metrics.Where(a => a.ColaboradorId.ToString().Equals(pessoa.Id.ToString(), StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(a.ParentType))
                    .GroupBy(a => new { a.ParentType, a.ParentTitulo })
                    .Select(a => new ParentItemView()
                    {
                        Tipo = a.Key.ParentType,
                        Titulo = a.Key.ParentTitulo,
                        Horas = metricas.GetHoras(a.Select(c => c.Duracao))
                    })
                    .OrderByDescending(a => a.Horas)
                    .ToArray();

                if (peneira.Length > 6)
                {
                    var atvdRestantes = peneira.Length - 6;
                    colaborador.Parents = peneira.Take(6).ToArray();
                    colaborador.Menssagem = $"+{atvdRestantes} Atividade(s)";
                }
                else
                {
                    colaborador.Parents = peneira;
                    colaborador.Menssagem = $"{peneira.Length} Atividade(s) Ativas";

            }


            colaborador.PJ = pessoa.CargaHoraria > 0;

                AddEngajamento(id, colaborador);

                return colaborador;
            
        }
        private List<EvolucaoEngajamentoView> AddEngajamento(string id, ColaboradorDashboardView colaborador)
        {
            //var anterior1 = 0; var anterior2 = 0;

            switch (id)
            {
                case "69DB13EF-89C0-4A6F-D71F-08DC62DFD032": // Amanda Ferreira
                    colaborador.HE_Individual = "03:54";
                    //anterior1 = 99;
                    //anterior2 = 95;
                    break;

                case "BD984996-9C11-4095-D71D-08DC62DFD032": // Fernanda Cassiano
                    colaborador.HE_Individual = colaborador.TotalPonto.ToString();
                    //anterior1 = 78;
                    //anterior2 = 0;
                    break;

                case "52F14677-9C85-41D5-D723-08DC62DFD032": // João Paulo
                    colaborador.HE_Individual = colaborador.TotalPonto.ToString();
                    //anterior1 = 100;
                    //anterior2 = 92;
                    break;

                case "C44D7319-3318-43D4-D726-08DC62DFD032": // Joel Martins
                    colaborador.HE_Individual = "01:56";
                    //anterior1 = 94;
                    //anterior2 = 90;
                    break;

                case "3F7E1A71-815A-4397-D725-08DC62DFD032": // Junior Dias
                    colaborador.HE_Individual = "-01:18"; ;
                    //anterior1 = 79;
                    //anterior2 = 96;
                    break;

                case "0D6227A1-7B72-4DAC-D720-08DC62DFD032": // Luiz Oliveira
                    colaborador.HE_Individual = colaborador.TotalPonto.ToString(); ;
                    //anterior1 = 75;
                    //anterior2 = 100;
                    break;

                case "C0D4394F-38EF-4F8B-D71E-08DC62DFD032": // Paulo Silva
                    colaborador.HE_Individual = "03:11";
                    //anterior1 = 98;
                    //anterior2 = 99;
                    break;

                case "87B833CD-7810-4030-D722-08DC62DFD032": // Thiago Oliveira
                    colaborador.HE_Individual = "16:39";
                    //anterior1 = 100;
                    //anterior2 = 97;
                    break;

                case "11B207E8-E5F6-44B6-32CA-08DC9125DFEC": // Petrônio Aleixo
                    colaborador.HE_Individual = "01:37";
                    //anterior1 = 90;
                    //anterior2 = 89;
                    break;
            }

            var lista = new List<EvolucaoEngajamentoView>();

            //lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(28, 26), Valor = anterior1 });
            //lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(14, 13), Valor = anterior2 });
            //lista.Add(new EvolucaoEngajamentoView() { Data = Periodo.SegundaFeiraPassada(7, 6), Valor = colaborador.Engajamento });

            colaborador.EvolucaoEngajamento = lista.ToArray();

            return lista;
        }
        
    }
}

