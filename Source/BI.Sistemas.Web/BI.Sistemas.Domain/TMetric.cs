using BI.Sistemas.Domain.Entities.Enums;
using System.Text.RegularExpressions;


namespace BI.Sistemas.Domain
{
    public class TMetric : EntityPeriodo
    {
        public DateTime Data { get; set; }
        public string Usuario { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Termino { get; set; }
        public DateTime Duracao { get; set; }
        public int? DevopsTask { get; set; }
        public string Atividade { get; set; }
        public Guid? ColaboradorId { get; set; }
        public virtual Colaborador Colaborador { get; set; }
        public DateTime DataCarga { get; set; }
        public string? Tipo { get; set; }
        public string? ParentType{ get; set; }
        public string? ParentTitulo { get; set; }

        public static TMetric FromCsv(DateTime dataCarga, string csvLine)
        {
            string[] values = Regex.Split(csvLine, ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            var data = Convert.ToDateTime(values[0]);
            var inicioCsv = Convert.ToDateTime(values[8]);
            var inicio = data.Date
                .AddHours(inicioCsv.Hour)
                .AddMinutes(inicioCsv.Minute)
                .AddSeconds(inicioCsv.Second);

            var terminoCsv = Convert.ToDateTime(values[9]);
            var termino = data.Date
                .AddHours(terminoCsv.Hour)
                .AddMinutes(terminoCsv.Minute)
                .AddSeconds(terminoCsv.Second);

            var duracaoCsv = Convert.ToDateTime(values[10]);
            var duracao = (new DateTime(1900, 1, 1))
                .AddHours(duracaoCsv.Hour)
                .AddMinutes(duracaoCsv.Minute)
                .AddSeconds(duracaoCsv.Second);

            var tipo = values[7];

            var task = values[11]?.Replace("#", "");

            var dailyValues = new TMetric();
            dailyValues.Data = data;
            dailyValues.Usuario = values[1];
            dailyValues.Inicio = inicio;
            dailyValues.Termino = termino;
            dailyValues.Duracao = duracao;
            dailyValues.DevopsTask = string.IsNullOrEmpty(task) ? default(int?) : Convert.ToInt32(task);
            dailyValues.Atividade = values[2];
            dailyValues.DataCarga = dataCarga;
            dailyValues.Tipo = values[7];
            dailyValues.ParentType = string.IsNullOrWhiteSpace(values[13]) ? default(string?) : values[13];
            dailyValues.ParentTitulo = string.IsNullOrWhiteSpace(values[14]) ? default(string?) : values[14];

            return dailyValues;
        }
        public double GetHoras(IEnumerable<DateTime> dates)
        {
            var total = new TimeSpan();

            foreach (var item in dates.Select(c => c.TimeOfDay))
                total = total.Add(item);

            return total.TotalHours;
        }
        public int CalcularEngajamento(IEnumerable<TMetric> tmetrics, IEnumerable<Ponto> pontos, Colaborador colaborador)
        {
            var hora = GetHoras(tmetrics.Where(c => c.ColaboradorId.ToString().ToUpper() == colaborador.Id.ToString()
            .ToUpper()).Select(h => h.Duracao));
            var ponto = GetHoras(pontos.Where(p => p.ColaboradorId.ToString().ToUpper() == colaborador.Id.ToString()
            .ToUpper()).Select(h => h.Horas));

            if (ponto == 0 && colaborador.CargaHoraria > 0)
            {
                ponto = colaborador.CargaHoraria;
                if (ponto < hora)
                    ponto = hora;
            }
            return ponto == 0 ? 0 : (int)Math.Round(hora / ponto * 100, 0);
            
        }
    }
}
