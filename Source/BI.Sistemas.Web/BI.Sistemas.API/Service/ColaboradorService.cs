using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain.Entities.Enums;
using BI.Sistemas.Domain;
using Microsoft.IdentityModel.Tokens;
using static BI.Sistemas.API.View.ColaboradorDashboardView;
using BI.Sistemas.Domain.Extensions;
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
        public ColaboradorDashboardView GetColaboradorDashboard(string id)
        {
            TMetric metricas = new TMetric();
            if (id.IsNullOrEmpty())
                throw new Exception("O ID do colaborador não foi fornecido!");

            var pontosTodos = _colaboradorRepository.GetPonto();
            var tmetricTodos = _colaboradorRepository.GetTodos();
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
                .Where(he => he.PeriodoId.ToString().Equals(HE.PeriodoId.ToString(), StringComparison.CurrentCultureIgnoreCase) && he.ColaboradorId
                .ToString().Equals(id, StringComparison.OrdinalIgnoreCase) && he.Tipo == TipoPonto.Normal)
                .Select(C => C.Horas);

            colaborador.TotalPonto = HoraPonto != null && HoraPonto.Any() ? metricas.GetHoras(HoraPonto) : pessoa.CargaHoraria;


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
            

            var filtro = metrics.Where(a => a.ColaboradorId.ToString().Equals(pessoa.Id.ToString(), StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(a.ParentType))
                .GroupBy(a => new { a.ParentType, a.ParentTitulo})
                .Select(a => new ParentItemView()
                {
                    Tipo = a.Key.ParentType,
                    Titulo = a.Key.ParentTitulo.Length > 60 ? $"{a.Key.ParentTitulo.Substring(0, 60)} ..." : a.Key.ParentTitulo,
                    Horas = metricas.GetHoras(a.Select(c => c.Duracao))
                })
                .OrderByDescending(a => a.Horas)
                .ToArray();

            if (filtro.Length > 5)
            {
                var atvdRestantes = filtro.Length - 5;
                colaborador.Parents = filtro.Take(5).ToArray();
                colaborador.Menssagem = $"+{atvdRestantes} Atividade(s) Ativas";
            }
            else
            {
                colaborador.Parents = filtro;
                colaborador.Menssagem = $"{filtro.Length} Atividade(s) Ativas";
            }

            colaborador.PJ = pessoa.CargaHoraria > 0;
            var he = _colaboradorRepository.GetHE(pessoa, periodoAtual);
            AddEngajamento(id, colaborador, he);

            return colaborador;

        }
        private List<EvolucaoEngajamentoView> AddEngajamento(string id, ColaboradorDashboardView colaborador, HE? he)
        {

            if (!colaborador.PJ && he != null)
            {
                _colaboradorRepository.GetPessoa(id);
                colaborador.HE_Individual = he.Horas;
            }
            else
            {
                _colaboradorRepository.GetPessoa(id);
                colaborador.HE_Individual = (decimal)colaborador.TotalPonto;
            }

            var lista = new List<EvolucaoEngajamentoView>();

            colaborador.EvolucaoEngajamento = lista.ToArray();

            return lista;
        }
    }
}

