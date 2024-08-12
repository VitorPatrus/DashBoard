namespace BI.Sistemas.Domain
{
    public class Periodo : Entity
    {
        public DateTime Data { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termino { get; set; }

        public string SegundaPassada()
        {
            DateTime dataAtual = DateTime.Now;
            DayOfWeek diaAtual = dataAtual.DayOfWeek;
            int diasParaSubtrair = (int)diaAtual + 6;
            DateTime segundaFeiraPassada = dataAtual.AddDays(-diasParaSubtrair);
            string segundaFormatada = segundaFeiraPassada.ToString("dd/MM");

            return segundaFormatada;

        }
    }
}
