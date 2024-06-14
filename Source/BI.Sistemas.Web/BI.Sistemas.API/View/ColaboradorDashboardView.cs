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
        public string HE_Individual { get; set; }
        public string HE_Equipe { get; set; }
        public bool PJ { get; set; }
        public double TotalPonto { get; set; }
        public EngajamentoView[] TopEngajamento { get; set; }
        public AtividadeView[] Atividades { get; set; }
        public EvolucaoEngajamentoView[] EvolucaoEngajamento { get; set; }


        public double TotalApropriado
        {
            // Propriedade que calcula o total de horas apropriadas para todas as atividades.
            get
            {
                return Atividades
                   .Sum (a => a.Horas.TimeOfDay.TotalHours); // Soma das horas de todas as atividades.
            }
        }
        public double DevOps
        {
            get
            {
                var atividadeDevOps = Atividades.Where(a => a.Tipo == "Bug" || a.Tipo == "Delivery" || a.Tipo == "Discovery");


                return atividadeDevOps
                    .Where(a => ! string.IsNullOrEmpty(a.Ticket))
                    .Sum(a => a.Horas.TimeOfDay.TotalHours)/
                    atividadeDevOps.Sum(a => a.Horas.TimeOfDay.TotalHours) * 100;
            }
        }
        


        public ApropriacaoView[] ResumoApropriacao
        {
            // Propriedade que gera um resumo das horas apropriadas agrupadas por tipo de atividade.
            get
            {
                return Atividades
                    .GroupBy(a => a.Tipo) // Agrupa as atividades pelo tipo.
                    .Select(a => new ApropriacaoView() { Tipo = a.Key, Valor = a.Sum(a => a.Horas.TimeOfDay.TotalHours) }) // Para cada grupo, cria uma nova instância de ApropriacaoView com o tipo e o total de horas apropriadas.
                    .ToArray(); // Converte a sequência de resultados em uma matriz.
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
            
            public string Color
            {
                get
                {
                    if (Tipo == "Delivery")
                    {
                        return "#00b050";
                    }
                    else if (Tipo == "Bug")
                    {
                        return "#ff0000";
                    }
                    else if (Tipo == "Ceremony")
                    {
                        return "#e49edd";
                    }
                    else if (Tipo == "Discovery")
                    {
                        return "#00b0f0";
                    }
                    else if (Tipo == "Coaching")
                    {
                        return "#782170";
                    }
                    else if (Tipo == "Out Of Office")
                    {
                        return "#ffc000";
                    }
                    else if (Tipo == "Management")
                    {
                        return "#f6a700";
                    }
                    return string.Empty;
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
