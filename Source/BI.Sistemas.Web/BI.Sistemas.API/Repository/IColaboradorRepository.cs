using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;

namespace BI.Sistemas.API.Repository
{
    public interface IColaboradorRepository
    {
        BISistemasContext GetContext();
        List<TMetric> GetTodos();
        Colaborador GetById(string id);
        Colaborador GetPessoa(string id);
        List<Ponto> GetPonto();
        Periodo GetPeriodo();
        List<Colaborador> GetListaColaboradores();
        List<Colaborador> GetListaColaboradoresSuporte();
        List<Movidesk> GetChamados(Periodo periodo);
        List<Colaborador> GetColaboradoresTestes();
        void AddColaborador(BISistemasContext db, IEnumerable<Colaborador> colaboradores, string nome, string cargo, string time, string userTMetric, string? fileNameFoto = null);
    }
}

