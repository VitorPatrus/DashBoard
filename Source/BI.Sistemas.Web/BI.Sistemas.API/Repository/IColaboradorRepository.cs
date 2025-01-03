using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;

namespace BI.Sistemas.API.Repository
{
    public interface IColaboradorRepository
    {
        List<TMetric> GetTodos();
        Colaborador GetPessoa(string id);
        List<Ponto> GetPonto();
        Periodo GetPeriodo();
        List<Colaborador> GetListaColaboradores();
        List<Movidesk> GetChamados(Periodo periodo);
        List<Colaborador> GetColaboradoresTestes();
        HE? GetHE(Colaborador colaborador, Periodo periodoAtual);
    }
}

