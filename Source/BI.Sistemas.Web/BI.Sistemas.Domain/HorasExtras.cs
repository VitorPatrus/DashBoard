using BI.Sistemas.Domain;

namespace BI.Sistemas.Context
{
    public class HorasExtras
    {
        public Guid ID { get; set; }
        public string Colaborador { get; set; }
        public decimal Horas { get; set; }
        public DateTime? Data { get; set; }
        public Guid Periodo { get; set; }
    }
}