using BI.Sistemas.API.Repository;
using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Extensions;
using BI.Sistemas.Domain.Novo;
using Microsoft.Extensions.WebEncoders.Testing;
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

            //colaborador.LeadTime = CalcularPercentual(chamadosDaPessoa.Sum(x => x.LeadTime ?? 0), chamadosDaPessoa.Where(x => x.LeadTime.HasValue).Count());
            //colaborador.LeadTimeEquipe = CalcularPercentual(chamadosEquipe.Sum(x => x.LeadTime ?? 0), chamadosEquipe.Where(x => x.LeadTime.HasValue).Count());
            //colaborador.LeadTimeSistemas = CalcularPercentual(chamados.Sum(x => x.LeadTime ?? 0), chamados.Where(x => x.LeadTime.HasValue).Count());

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
                    DataFechamento = x.DataFechamento?.ToString("dd/MM/yyyy"),
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

            var sistemas = chamados.Where(x => !x.EstaFechado).ToList();
            colaborador.Sistemas = sistemas.Count();

            var chamadosIndividual = chamadosDaPessoa.Count();

            int totalChamadosEquipe = chamadosEquipe.Count();
            int equipeNoPrazo = chamadosEquipe.Count(x => x.IndicadorSLA == "No Prazo");
            int equipeForaPrazo = chamadosEquipe.Count(x => x.IndicadorSLA == "Fora do Prazo");

            double porcentagemTime = totalChamadosEquipe > 0 ? CalcularPercentual(equipeNoPrazo, equipeNoPrazo + equipeForaPrazo) : 0;
            colaborador.SLA_Time = porcentagemTime;

            var listaColaboradoresOrdenados = chamados
            .Where(x => colaboradoresSuporte.Any(c => c.Id.EqualsGuid(x.ResponsavelId.ToString())))
            .GroupBy(c => c.ResponsavelChamado)
            .Select(x => new
            {
                Nome = x.Key,
                DentroDoPrazo = x.Count(y => y.IndicadorSLA == "No Prazo"),
                ForaDoPrazo = x.Count(y => y.IndicadorSLA == "Fora do Prazo")
            })
            .OrderByDescending(q => q.DentroDoPrazo / (q.DentroDoPrazo + q.ForaDoPrazo) * 100)
            .ThenByDescending(x => x.DentroDoPrazo)
            .Where(x => x.DentroDoPrazo > 0)
            .ToList();

            colaborador.TopSLA = listaColaboradoresOrdenados
             .Select(l => new SLAView
             {
                 Nome = l.Nome,
                 Percentual = CalcularPercentual(l.DentroDoPrazo, l.DentroDoPrazo + l.ForaDoPrazo),
                 DentroDoPrazo = l.DentroDoPrazo
             })
             .Take(3)
            .ToList();

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

        foreach (var obj in chamados)
        {
            if (obj.ResponsavelChamado != "")
            {
                if (obj.DataFechamento != null)
                {
                    var abertura = obj?.DataAbertura;
                    var fechamento = obj?.DataFechamento;
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
            }
        }
        return soma.ToString("F0");
    }
    private List<EvolucaoSLAView> AddEngajamento(string id, ColaboradorSLADashboardView colaborador)
    {
        id = id.ToUpper();
        switch (id)
        {
            case "E766B8ED-DB49-4198-EFFA-08DCBC682A35": // Anna Paula Gomes da Silva
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "91C9FA96-B633-44DA-EFFF-08DCBC682A35": // Arthur Abreu
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "4D143095-82BC-42C5-EFFE-08DCBC682A35": // Barbara Barros
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "9C1BC7C9-61E1-4D9B-EFF8-08DCBC682A35": // Euller Neviton Vieira
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "FCE1ADC9-3227-461D-EFF4-08DCBC682A35": // Giovanni de Souza Campos
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "81AF4E8A-9AE4-4EDC-EFF6-08DCBC682A35": // Isaias Oliveira Guimaraes
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "64290DD1-1C7F-449F-EFFB-08DCBC682A35": // Joao Pedro Martins dos Santos
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "0AB1696C-79EA-47D5-F005-08DCBC682A35": // Joselito Almeida
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "53E72B2C-A573-4F88-F001-08DCBC682A35": // Kleverson Salles
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "8242B481-8791-4250-F004-08DCBC682A35": // Marcus Ethur
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "DB28ECCD-C692-45CD-F003-08DCBC682A35": // Marcus Vinícius
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "196A275C-1C5B-4DEE-EFF5-08DCBC682A35": // Mateus de Oliveira Menezes Aquino
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "7D03BF80-6667-49D6-F002-08DCBC682A35": // Natalia Caroline
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "D0D84A4B-3939-49E1-EFF9-08DCBC682A35": // Paulo Junior Souza Ramos
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "096E2CF9-E24E-4B69-EFFC-08DCBC682A35": // Ricardo Rodrigues dos Santos
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "55C23E66-842C-453F-EFFD-08DCBC682A35": // Samuel Rodrigo Lopes Ferreira
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;

            case "D899C604-AF30-4697-EFF7-08DCBC682A35": // Vitor dos Santos Gomes
                colaborador.EvolucaoChamadosAbertos = new int[] { 00, 00, 00, colaborador.Pessoal };
                colaborador.EvolucaoChamadosFechados = new int[] { 00, 00, 00, colaborador.FechadosPessoal };
                break;
        }


        var lista = new List<EvolucaoSLAView>();

        for (int i = 0; i < colaborador.EvolucaoChamadosAbertos.Length; i++)
        {
            lista.Add(new EvolucaoSLAView()
            {
                Data = Periodo.SegundaFeiraPassada(7, 6),
                Valor = colaborador.EvolucaoChamadosAbertos[i]
            });
            lista.Add(new EvolucaoSLAView()
            {
                Data = Periodo.SegundaFeiraPassada(14, 13), 
                Valor = colaborador.EvolucaoChamadosFechados[i]
            });
            lista.Add(new EvolucaoSLAView()
            {
                Data = Periodo.SegundaFeiraPassada(21, 20), 
                Valor = colaborador.EvolucaoChamadosFechados[i]
            });
            lista.Add(new EvolucaoSLAView()
            {
                Data = Periodo.SegundaFeiraPassada(28, 27), 
                Valor = colaborador.EvolucaoChamadosFechados[i]
            });
        }

        return lista;
    }
    private int CalcularPercentual(int valor, int total)
    {
        if (total <= 0) return 0;
        return (int)Math.Round((double)valor / total * 100);
    }
}