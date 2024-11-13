using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Entities.Enums;
using System.Text.RegularExpressions;

namespace BI.Sistemas.API.View
{
    public class ColaboradorDashboardView
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string FotoColaborador { get; set; }
        public string FotoTime { get; set; }
        public string Cargo { get; set; }
        public string Time { get; set; }
        public int Engajamento { get; set; }
        public int EngajamentoTime { get; set; }
        public decimal? HE_Individual { get; set; }
        public string HE_Equipe { get; set; }
        public bool PJ { get; set; }
        public double TotalPonto { get; set; }
        public EngajamentoView[] TopEngajamento { get; set; }
        public AtividadeView[] Atividades { get; set; }
        public EvolucaoEngajamentoView[] EvolucaoEngajamento { get; set; }
        public ParentItemView[] Parents { get; set; }
        public string Menssagem { get; set; }

        public double TotalApropriado
        {
            get
            {
                return Atividades
                   .Sum(a => a.Horas.TimeOfDay.TotalHours);
            }
        }
        public double DevOps
        {
            get
            {
                var atividadeDevOps = Atividades.Where(a => a.Tipo == "Bug" || a.Tipo == "Delivery" || a.Tipo == "Discovery");


                return atividadeDevOps
                    .Where(a => !string.IsNullOrEmpty(a.Ticket))
                    .Sum(a => a.Horas.TimeOfDay.TotalHours) /
                    atividadeDevOps.Sum(a => a.Horas.TimeOfDay.TotalHours) * 100;
            }
        }

        public ApropriacaoView[] ResumoApropriacao
        {
            get
            {
                return Atividades
                    .GroupBy(a => a.Tipo)
                    .Select(a => new ApropriacaoView()
                    {
                        Tipo = a.Key,
                        Valor = (int)Math.Ceiling(a.Sum(a => a.Horas.TimeOfDay.TotalHours)),
                        Horas = (int)Math.Ceiling(a.Sum(a => a.Horas.TimeOfDay.TotalHours)),
                    }).ToArray();
            }

        }

        public class EngajamentoView
        {
            public string Nome { get; set; }
            public string Foto { get; set; }
            public int Percentual { get; set; }

        }
        public class EvolucaoEngajamentoView
        {
            public string Data { get; set; }
            public int Valor { get; set; }
        }
        public class ApropriacaoView
        {
            public string Tipo { get; set; }
            public double Valor { get; set; }
            public double Horas { get; set; }

            public string Color
            {
                get
                {
                    string cor;

                    switch (Tipo)
                    {
                        case "Delivery":
                            cor = "#00b050"; // Verde
                            break;
                        case "Bug":
                            cor = "#ff0000"; // Vermelho
                            break;
                        case "Ceremony":
                            cor = "#e49edd"; // Rosa
                            break;
                        case "Discovery":
                            cor = "#00b0f0"; // Azul
                            break;
                        case "Coaching":
                            cor = "#782170"; // Roxo
                            break;
                        case "Out Of Office":
                            cor = "#ffc000"; // Laranja
                            break;
                        case "Management":
                            cor = "#f6a700"; // Amarelo
                            break;
                        default:
                            cor = string.Empty;
                            break;
                    }

                    return cor;
                }
            }
        }
        public class AtividadeView
        {
            public string? Atividade { get; set; }
            public string? Ticket { get; set; }
            public string? TicketLink { get; set; }
            public string? Tipo { get; set; }
            public string? Data { get; set; }
            public DateTime Horas { get; set; }
            public double DevOps { get; set; }
        }
    }
}
