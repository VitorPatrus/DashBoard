using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Novo
{
    public class GeracaoBoard
    {
        public DateTime Data;
        public DateTime DataInicio;
        public DateTime DataFim;

        public GeracaoBoard() { }

        public GeracaoBoard(DateTime data, DateTime dataInicio, DateTime dataFim)
        {
            Data = data;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }
    }
}
