using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;
using Microsoft.EntityFrameworkCore;

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
        public Colaborador GetById(string id)
        {
            return _dbcontext.Colaboradores.FirstOrDefault(x => x.Id.ToString().Trim().Equals(id.ToString().Trim(), StringComparison.OrdinalIgnoreCase));
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
        public List<Colaborador> GetListaColaboradoresSuporte()
        {
            return _dbcontext.Colaboradores.Where(x => x.Suporte).ToList();
        }
        public List<Movidesk> GetChamados(Periodo periodo)
        {
            return _dbcontext.Movidesks.Where(x => x.Periodo == periodo).ToList();
        }
        public List<Colaborador> GetColaboradoresTestes()
        {
            return _dbcontext.Colaboradores.AsQueryable().ToList();
        }
        public void AddColaborador(BISistemasContext db,
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

        public BISistemasContext GetContext()
        {
            return _dbcontext;
        }
    }
}
