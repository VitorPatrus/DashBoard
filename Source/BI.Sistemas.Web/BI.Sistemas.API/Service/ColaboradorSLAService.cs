using BI.Sistemas.API.Repository;
using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static BI.Sistemas.API.View.ColaboradorSLADashboardView;

public class ColaboradorSLAService : IColaboradorSLAService
{
    private readonly IColaboradorRepository _colaboradorRepository;
    public ColaboradorSLAService(IColaboradorRepository colaboradorRepository)
    {
        _colaboradorRepository = colaboradorRepository;
    }
    public ColaboradorSLADashboardView GetColaboradorDashboard(string id)
    {

        try
        {
            if (id.IsNullOrEmpty())
                throw new Exception("O ID do colaborador não foi fornecido!");

            var pessoa = _colaboradorRepository.GetPessoa(id);

            if (pessoa == null)
                throw new Exception($"Colaborador não encontrado (ID: {id})");

            var periodo = _colaboradorRepository.GetPeriodo();
            var colaboradoresSuporte = _colaboradorRepository.GetListaColaboradores().Where(x => x.Suporte).ToList();
            var chamados = _colaboradorRepository.GetChamados(periodo);
            var chamadosDaPessoa = chamados.Where(x => pessoa.UserTMetric.Trim().Equals(x.ResponsavelChamado.Trim(), StringComparison.CurrentCultureIgnoreCase)).ToList();
            var chamadosEquipe = chamados.Where(x => x.Time.Equals(pessoa.Time, StringComparison.CurrentCultureIgnoreCase)).ToList();
            var colaborador = new ColaboradorSLADashboardView();

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

            colaborador.LeadTime = CalcularLeadTime(colaborador, chamadosDaPessoa);
            colaborador.LeadTimeEquipe = CalcularLeadTime(colaborador, chamadosEquipe);
            colaborador.LeadTimeSistemas = CalcularLeadTime(colaborador, chamados);

            var tabelaForaPrazo = chamadosDaPessoa.Where(x => x.IndicadorSLA == "Fora do Prazo").ToList();
            colaborador.TabelaForaPrazo = tabelaForaPrazo
                .Select(x => new ChamadoView
                {
                    Numero = x.Numero,
                    Assunto = x.Assunto,
                    DataAbertura = x.DataAbertura?.ToString("dd/MM/yyyy"),
                    DataFechamento = x.DataVencimento?.ToString("dd/MM/yyyy"),
                    Servico = x.Servico,
                    Solicitante = x.Pessoa

                }).ToList();

            var tabelaPendentes = chamadosDaPessoa.Where(x => x.Status != "Fechado"
            && x.Status != "Cancelado"
            && x.Status != "Resolvido").ToList();

            colaborador.TabelaPendentes = tabelaPendentes
                .Select(x => new ChamadoView
                {
                    Numero = x.Numero,
                    Assunto = x.Assunto,
                    DataAbertura = x.DataAbertura?.ToString("dd/MM/yyyy"),
                    Servico = x.Servico,
                    Solicitante = x.Pessoa

                }).ToList();

            var servicos = chamadosDaPessoa.GroupBy(x => x.Servico)
                .OrderByDescending(x => x.Count())
                .Take(3).ToList();

            colaborador.Servicos = servicos
                .Select(x => new ServicoView
                {
                    Servico = x.Key,
                    Quantidade = x.Count()

                }).ToList();

            colaborador.Pessoal = chamadosDaPessoa.Count(x => !x.EstaFechado);
            colaborador.DentroPrazo = chamadosDaPessoa.Count(x => x.IndicadorSLA == "No Prazo");
            colaborador.ForaPrazo = chamadosDaPessoa.Count(x => x.IndicadorSLA == "Fora do Prazo");
            colaborador.Aguardando = chamadosDaPessoa.Count(x => x.Status.Contains("Aguardando"));

            colaborador.FechadosPessoal = chamadosDaPessoa.Count(x => x.EstaFechado);
            colaborador.FechadosEquipe = chamadosEquipe.Count(x => x.EstaFechado);
            colaborador.FechadosSistemas = chamados.Count(x => x.EstaFechado);
            colaborador.SLA_Individual = CalcularPercentual(colaborador.DentroPrazo, colaborador.ForaPrazo + colaborador.DentroPrazo);
            colaborador.Chamados_SLA_Individual = colaborador.ForaPrazo + colaborador.DentroPrazo;
            colaborador.Setorial = chamadosEquipe.Count(x => !x.EstaFechado);

            colaborador.Sistemas = chamados.Where(x => !x.EstaFechado).ToList().Count();

            var chamadosIndividual = chamadosDaPessoa.Count();

            int totalChamadosEquipe = chamadosEquipe.Count();
            int equipeNoPrazo = chamadosEquipe.Count(x => x.IndicadorSLA == "No Prazo");
            int equipeForaPrazo = chamadosEquipe.Count(x => x.IndicadorSLA == "Fora do Prazo");

            int sistemasForaPrazo = chamados.Count(x => x.IndicadorSLA == "Fora do Prazo");
            int sistemasNoPrazo = chamados.Count(x => x.IndicadorSLA == "No Prazo");

            colaborador.SLA_Sistemas = chamados.Count() > 0 ? CalcularPercentual(sistemasNoPrazo, sistemasNoPrazo + sistemasForaPrazo) : 0;
            colaborador.SLA_Time = totalChamadosEquipe > 0 ? CalcularPercentual(equipeNoPrazo, equipeNoPrazo + equipeForaPrazo) : 0;

            var listaColaboradoresOrdenados = chamados
            .Where(x => colaboradoresSuporte.Any(c => c.Nome.Equals(colaborador.Nome, StringComparison.CurrentCultureIgnoreCase)))
            .GroupBy(c => c.ResponsavelChamado)
            .Select(x => new
            {
                Nome = x.Key,
                DentroDoPrazo = x.Count(x => x.IndicadorSLA == "No Prazo"),
                ForaDoPrazo = x.Count(x => x.IndicadorSLA == "Fora do Prazo")
            })
            .OrderByDescending(q => q.DentroDoPrazo / (q.DentroDoPrazo + q.ForaDoPrazo) * 100)
            .ThenByDescending(x => x.DentroDoPrazo)
            .Where(x => x.DentroDoPrazo > 0)
            .ToList();

            var exclude = new List<string> { "", "Amanda Ferreira (TI MTZ)" };

            listaColaboradoresOrdenados.RemoveAll(e =>
                exclude.Contains(e.Nome, StringComparer.CurrentCultureIgnoreCase));

            colaborador.TopSLA = listaColaboradoresOrdenados
             .Select(l => new SLAView
             {
                 Nome = l.Nome,
                 Percentual = CalcularPercentual(l.DentroDoPrazo, l.DentroDoPrazo + l.ForaDoPrazo),
                 DentroDoPrazo = l.DentroDoPrazo
             })
             .Take(3)
            .ToList();

            colaborador.HE = _colaboradorRepository.GetHE(pessoa, periodoAtual);

            AddEngajamento(id, colaborador);

            return colaborador;
        }
        catch (DivideByZeroException)
        {
            throw new Exception("Divisão por 0");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private string CalcularLeadTime(ColaboradorSLADashboardView colaborador, List<Movidesk> chamados)
    {
        double soma = 0;
        double conta = 0;

        foreach (var obj in chamados)
        {
            if (obj.ResponsavelChamado != "")
            {
                if (obj.DataVencimento != null)
                {
                    var abertura = obj?.DataAbertura;
                    var fechamento = obj?.DataVencimento;
                    var diferenca = fechamento - abertura;
                    var dias = diferenca?.TotalDays;
                    soma += dias ?? 0;
                }
                else
                {
                    var abertura = obj?.DataAbertura;
                    var fechamento = DateTime.Today;
                    var diferenca = fechamento - abertura;
                    var dias = diferenca?.TotalDays;
                    soma += dias ?? 0;
                }
                conta++;
            }
        }
        return (soma / conta).ToString("F0");
    }
    private List<EvolucaoSLAView> AddEngajamento(string id, ColaboradorSLADashboardView colaborador)
    {
        var pessoa = _colaboradorRepository.GetPessoa(id.ToUpper());

        if (pessoa == null)
            throw new Exception("Colaborador não encontrado.");

        var periodoAtual = _colaboradorRepository.GetPeriodo();
        var evolucaoSLA = FiltrarEvolucao(id);

        colaborador.EvolucaoChamadosAbertos = new int[]
        {
        evolucaoSLA.Length > 3 ? evolucaoSLA[2].DentroDoPrazo : 0,
        evolucaoSLA.Length > 2 ? evolucaoSLA[1].DentroDoPrazo : 0,
        evolucaoSLA.Length > 1 ? evolucaoSLA[0].DentroDoPrazo : 0,
        colaborador.Pessoal
        };

        colaborador.EvolucaoChamadosFechados = new int[]
        {
        evolucaoSLA.Length > 2 ? evolucaoSLA[2].ForaDoPrazo : 0,
        evolucaoSLA.Length > 1 ? evolucaoSLA[1].ForaDoPrazo : 0,
        evolucaoSLA.Length > 0 ? evolucaoSLA[0].ForaDoPrazo : 0,
        colaborador.FechadosPessoal
        };

        var lista = new List<EvolucaoSLAView>();

        for (int i = 0; i < colaborador.EvolucaoChamadosAbertos.Length; i++)
        {
            lista.Add(new EvolucaoSLAView()
            {
                Valor = colaborador.EvolucaoChamadosAbertos[i]
            });
            lista.Add(new EvolucaoSLAView()
            {
                Valor = colaborador.EvolucaoChamadosFechados[i]
            });
        }

        AtualizarEvolucaoSLA(id, colaborador, periodoAtual);

        return lista;
    }

    public EvolucaoSLA[] FiltrarEvolucao(string id)
    {
        using (var db = new BISistemasContext())
        {
            var colaborador = db.Colaboradores.FirstOrDefault(c => c.Id.ToString().ToUpper() == id.ToUpper());

            if (colaborador != null)
            {
                return db.EvolucaoSLA
                    .Where(e => e.ColaboradorId == colaborador.Id)
                    .OrderByDescending(e => e.Data)
                    .Skip(2)
                    .Take(3)
                    .ToArray();
            }
            else
            {
                throw new Exception();
            }
        }
    }

    public void AtualizarEvolucaoSLA(string id, ColaboradorSLADashboardView colaboradorView, Periodo periodoAtual)
    {
        var pessoa = _colaboradorRepository.GetPessoa(id);
        var lista = new List<EvolucaoSLA>();

        using (var db = new BISistemasContext())
        {
            var colaborador = db.Colaboradores.FirstOrDefault(c => c.Id.ToString().ToUpper() == id.ToUpper());

            if (colaborador != null)
            {
                var registroExistente = db.EvolucaoSLA
                    .FirstOrDefault(e => e.ColaboradorId == colaborador.Id && e.PeriodoId == periodoAtual.Id);

                if (registroExistente == null)
                {
                    var novoSla = new EvolucaoSLA
                    {
                        ColaboradorId = colaborador.Id,
                        DentroDoPrazo = colaboradorView.Pessoal,
                        ForaDoPrazo = colaboradorView.FechadosPessoal,
                        Data = DateTime.Today,
                        PeriodoId = periodoAtual.Id
                    };

                    db.EvolucaoSLA.Add(novoSla);
                }
                else
                {
                    registroExistente.DentroDoPrazo = colaboradorView.Pessoal;
                    registroExistente.ForaDoPrazo = colaboradorView.FechadosPessoal;
                    registroExistente.Data = DateTime.Today;

                    db.Entry(registroExistente).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            else
            {
                throw new Exception("Colaborador não encontrado.");
            }
        }
    }

    private static int CalcularPercentual(int valor, int total)
    {
        var retorno = (total <= 0) ? 0 : (int)Math.Round((double)valor / total * 100);
        return retorno;
    }
}