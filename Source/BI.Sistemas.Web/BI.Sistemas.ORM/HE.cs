using BI.Sistemas.Domain;

namespace BI.Sistemas.Context
{
    public class HE
    {
        public Guid Id { get; set; }
        public Guid ColaboradorId { get; set; } 
        public decimal Horas { get; set; } 
        public DateTime Data { get; set; }
        public Guid PeriodoId { get; set; } 

        
        public Colaborador Colaborador { get; set; }
        public Periodo Periodo { get; set; }
    }

}