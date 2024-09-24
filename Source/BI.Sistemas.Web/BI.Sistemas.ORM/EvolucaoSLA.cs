using BI.Sistemas.Domain;

namespace BI.Sistemas.Context
{
    public class EvolucaoSLA
    {
        public Guid Id { get; set; }
        public Guid ColaboradorId { get; set; }
        public int DentroDoPrazo { get; set; }
        public int ForaDoPrazo { get; set; }
        public DateTime Data { get; set; }
        public Guid PeriodoId { get; set; }


        public Colaborador Colaborador { get; set; }
        public Periodo Periodo { get; set; }
    }
}