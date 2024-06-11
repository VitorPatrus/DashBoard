namespace BI.Sistemas.Domain
{
    public class Periodo : Entity
    {
        public DateTime Data { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termino { get; set; }
    }
}
