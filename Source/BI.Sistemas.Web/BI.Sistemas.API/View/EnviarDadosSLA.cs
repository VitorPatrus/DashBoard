using static BI.Sistemas.API.View.ColaboradorDashboardView;

namespace BI.Sistemas.API.View
{
    public class EnviarDadosSLA
    {
        internal bool Oficial;

        public string Foto { get; set; }
        public string Id { get; set; }
            //public string Periodo { get; set; }
        public int SLA { get; set; }
            //public double DevOps { get; set; }
            //public bool Oficial { get; set; }
        public AtividadeView[] ListaForaPrazo { get; set; }
        //public AtividadeView[] ListaPendentes { get; set; }

    }
}
