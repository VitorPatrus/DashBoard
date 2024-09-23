using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Novo
{
    public enum AtividadeClassificacao
    {
        Bug = 1,
        Discovery = 2,
        Delivery = 3,
        Ceremony = 4,
        Coaching = 5,
        OutOfOffice = 6,
        Management = 7,

    }
    public class Atividades
    {
        public GeracaoColaborador Geracao { get; set; }

        public AtividadeClassificacao Classificacao { get; set; }

        public GeracaoColaborador GeracaoColaborador;
        public Atividades(GeracaoColaborador geracaoColaborador)
        {
            this.GeracaoColaborador = geracaoColaborador;

        }
        public GeracaoColaborador getGeracaoColaborador()
        {
            return GeracaoColaborador;

        }
    }
}
