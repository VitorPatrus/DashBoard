using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Novo
{
    public class GeracaoColaborador
    {
        public Colaborador Colaborador { get; }
        public Time Time { get; }
        public GeracaoTime GeracaoTime { get; set; }
        public double HE { get; set; }
        public double Engajamento { get; set; }
        public double TempoTrabalhado { get; set; }
        public double TempoApropriado { get; set; }
        public GeracaoColaborador() { }
        public GeracaoColaborador(Colaborador colaborador, Time time)
        {
            Colaborador = colaborador;
            Time = time;

        }
        public string GetColaboradorNome()
        {
            return Colaborador?.Nome;
        }
        public string GetTime()
        {
            return Time?.Nome;
        }
    }

}
