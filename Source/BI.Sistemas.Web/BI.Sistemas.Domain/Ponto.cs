using System.Text.RegularExpressions;
using BI.Sistemas.Domain.Entities.Enums;

namespace BI.Sistemas.Domain
{
    public class Ponto : EntityPeriodo
    {
        public DateTime Horas { get; set; }
        public Guid? ColaboradorId { get; set; }
        public virtual Colaborador Colaborador { get; set; }
        public TipoPonto Tipo { get; set; }
        public static Ponto FromCsv(IEnumerable<Colaborador> colaboradores, string csvLine)
        {
            string[] values = Regex.Split(csvLine, ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            var ponto = new Ponto();
            ponto.Tipo = TipoPonto.Normal;
            ponto.Colaborador = colaboradores.First(c => string.Compare(c.UserTMetric.ToUpper(), values[0].ToUpper(),true)==0);
            ponto.Horas= Convert.ToDateTime(values[5]);
            return ponto;
        }
    }
}
