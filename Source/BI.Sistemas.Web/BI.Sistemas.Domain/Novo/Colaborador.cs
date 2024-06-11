using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Novo
{
    public class Colaborador
    {
        public string Nome { get; set; }
        public string Foto { get; set; }
        public Time Time { get; set; }

        public Colaborador(Time time)
        {
            Time = time;
        }

        public String getNomeTime()
        {
            return Time.Nome;
        }
    }
}
