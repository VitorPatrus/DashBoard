using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;

namespace BI.Sistemas.API.Repository
{
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly BISistemasContext _dbcontext;
        public ColaboradorRepository(BISistemasContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public List<TMetric> GetTodos()
        {
            return _dbcontext.TMetrics.Where(a =>
                a.Usuario != "Marco Aurelio de Barros" &&
                a.Usuario != "Andre Costa (TI MTZ)" &&
                a.Usuario != "Marco Tulio Rodrigues").ToList();
        }

        public Colaborador GetPessoa(string id)
        {
            return _dbcontext.Colaboradores.FirstOrDefault(c => c.Id.ToString() == id);
        }

        public List<Ponto> GetPonto()
        {
            return _dbcontext.Pontos.ToList();
        }

        public Periodo GetPeriodo()
        {
            return _dbcontext.Periodos.OrderBy(x => x.Data).Last();
        }
        public List<Colaborador> GetListaColaboradores()
        {
            return _dbcontext.Colaboradores.ToList();
        }
        public List<Movidesk> GetChamados(Periodo periodo)
        {
            return _dbcontext.Movidesks.Where(x => x.Periodo == periodo).ToList();
        }
        public List<Colaborador> GetColaboradoresTestes()
        {
            return _dbcontext.Colaboradores.AsQueryable().ToList();
        }

        public HE? GetHE(Colaborador colaborador, Periodo periodo)
        {
            return _dbcontext.HorasExtras.FirstOrDefault(x => x.Colaborador == colaborador && x.Periodo == periodo);
        }
    }
}
