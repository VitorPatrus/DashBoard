using System.Diagnostics;

namespace BI.Sistemas.Domain
{
    public class Periodo : Entity
    {
        public DateTime Data { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termino { get; set; }

        public static string SegundaFeiraPassada(int diasParaInicioSemana, int diasVoltarAteSegunda)
        {
            DateTime dataAtual = DateTime.Now;
            DayOfWeek diaAtual = dataAtual.DayOfWeek;

            int diasParaSubtrair;
            if (diaAtual == DayOfWeek.Monday)
                diasParaSubtrair = diasParaInicioSemana;

            else
                diasParaSubtrair = (int)diaAtual + diasVoltarAteSegunda;

            DateTime segundaFeiraRetrasada = dataAtual.AddDays(-diasParaSubtrair);
            string segundaFormatada = segundaFeiraRetrasada.ToString("dd/MM");

            return segundaFormatada;
        }

    }
}
