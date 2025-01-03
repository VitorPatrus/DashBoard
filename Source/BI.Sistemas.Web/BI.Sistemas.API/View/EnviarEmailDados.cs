
namespace BI.Sistemas.API.View
{
    public class EnviarEmailDados
    {

        public string Foto { get; set; }
        public string Id { get; set; }
        public string Periodo { get; set; }
        public int Engajamento { get; set; }
        public double DevOps { get; set; }
        public bool Oficial { get; set; }
        public AtividadeView[] Lista { get; set; }

    }
}
