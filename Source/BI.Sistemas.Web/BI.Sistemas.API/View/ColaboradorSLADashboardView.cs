using BI.Sistemas.Domain;
using System.Text.RegularExpressions;

namespace BI.Sistemas.API.View
{
    public class ColaboradorSLADashboardView
    {
        public int ForaPrazo { get; set; }
        public int DentroPrazo { get; set; }
        public int Pessoal { get; set; }
        public int Setorial { get; set; }
        public int Sistemas { get; set; }
        public int HE_Compensavel { get; set; }
        public int HE_NaoCompensavel { get; set; }
        public int Pendentes { get; set; }
        public string FotoTime { get; set; }
        public string FotoColaborador { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cargo { get; set; }
        public string Time { get; set; }
        public double SLA_Individual { get; set; }
        public double SLA_Time { get; set; }
        public double TotalHoras { get; set; }
        public ChamadoView[] StatusChamado { get; set; }
        public List<SLAView> TopSLA { get; set; }
        //public List<ChamadoView> ForaPrazo { get; set; }
        //public List<ChamadoView> DentroPrazo { get; set; }
        //public List<ChamadoView> Pendentes { get; set; }

        public class SLAView
        {
            public string Nome { get; set; }
            public string Foto { get; set; }
            public int Percentual { get; set; }

        }
        public class ApropriacaoView
        {
            public string Tipo { get; set; }
            public double Valor { get; set; }
            public string Color
            {
                get
                {
                    if (Tipo == "Gestão Volume")
                    {
                        return "#00b050";
                    }
                    else if (Tipo == "SMP")
                    {
                        return "#ff0000";
                    }
                    else if (Tipo == "Nota Fiscal")
                    {
                        return "#e49edd";
                    }
                    else if (Tipo == "Tabela de Preço")
                    {
                        return "#00b0f0";
                    }
                    else if (Tipo == "CTE")
                    {
                        return "#782170";
                    }
                    return string.Empty;
                }
            }
        }
        public class ChamadoView
        {
            public Guid Id { get; set; }
            public string Ticket { get; set; }
            public string Atividade { get; set; }
            public string Tipo { get; set; }
            public DateTime Data { get; set; }
            public string Descricao { get; set; }
            public DateTime Horas { get; set; }

        }
    }
}
