using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Novo
{
    public class Movidesk : EntityPeriodo
    {
        public int Numero { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public DateTime? DataVencimento { get; set; }
        public string Pessoa { get; set; }
        public string Assunto { get; set; }
        public string IndicadorSLA { get; set; }
        public Colaborador? Responsavel { get; set; }
        public Guid? ResponsavelId { get; set; }
        public string ResponsavelChamado { get; set; }
        public string ResponsavelEquipe { get; set; }
        public string Servico { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }

        public Movidesk() { }


        public static Movidesk FromCsv(string csvLine)
        {
            string[] values = Regex.Split(csvLine, ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            var result = new Movidesk();
            result.Numero = Convert.ToInt32(values[0]);
            //var cultureInfo = new CultureInfo("en-US");
            result.DataAbertura = Convert.ToDateTime(values[1]);
            result.DataFechamento = string.IsNullOrEmpty(values[2]) ? default(DateTime?) : Convert.ToDateTime(values[2]);
            result.DataVencimento = string.IsNullOrEmpty(values[3]) || values[3].Equals("Em pausa", StringComparison.InvariantCultureIgnoreCase) ? default(DateTime?) : Convert.ToDateTime(values[3]);
            result.IndicadorSLA = values[4];
            result.Assunto = values[5];
            result.Pessoa = values[6];
            result.ResponsavelEquipe = values[7];
            result.ResponsavelChamado = values[8];
            result.Servico = values[9];
            result.Status = values[10];
            result.Time =

                result.ResponsavelEquipe.Equals("TI - EDI", StringComparison.InvariantCultureIgnoreCase) ? "EDI" :
                result.ResponsavelEquipe.Equals("TI - Sistemas CRM", StringComparison.InvariantCultureIgnoreCase) ? "CRM" :

                result.ResponsavelEquipe.Contains("TMS", StringComparison.InvariantCultureIgnoreCase) ? "Suporte TMS" :
                result.ResponsavelEquipe.Contains("ERP", StringComparison.InvariantCultureIgnoreCase) ? "Suporte ERP" :
                "Equipe não identificada. ";

            return result;


        }
    }

}

