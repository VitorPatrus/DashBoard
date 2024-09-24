using BI.Sistemas.API.Interfaces;
using BI.Sistemas.API.View;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Headers;

namespace BI.Sistemas.API.Service
{
    public class EconometroService : IEconometroService
    {
        private readonly IConfiguration _configuration;

        public EconometroService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public EconometroViewModel GetEconometroDashboard()
        {

            var model = new EconometroViewModel();
            try
            {
                var tokenTmetric = _configuration.GetSection("Econometro:TokenTmetric").Value ?? string.Empty;
                var userNameTMS = _configuration.GetSection("Econometro:UserNameTMS").Get<string[]>() ?? new string[0];
                var userNameERP = _configuration.GetSection("Econometro:UserNameERP").Get<string[]>() ?? new string[0];
                int.TryParse(_configuration.GetSection("Econometro:ValorHora").Value, out int valorHora);
                _configuration.GetSection("Econometro:Valores").Bind(model);
                var mesAtualAnalise = model.TotalAcumulado.Any() ?
                    model.TotalAcumulado.Select(c => DateTime.ParseExact($"01/{c.Chave}", "dd/MM/yyyy", CultureInfo.InvariantCulture)).Max().AddMonths(1) :
                    new DateTime(DateTime.Now.Year, 1, 1);
                var horasPorTime = new List<HorasPorTime>
                {
                    new HorasPorTime() { Hora = DateTime.Now }
                };
                var horaFinal = horasPorTime.Last().Hora.AddMinutes(10);
                while (horasPorTime.Last().Hora <= horaFinal)
                    horasPorTime.Add(new HorasPorTime() { Hora = horasPorTime.Last().Hora.AddSeconds(5) });

                var lancamentos = GetLancamentosTMetricAsync(tokenTmetric, mesAtualAnalise).Result;

                foreach (var item in lancamentos)
                {
                    if (!string.IsNullOrEmpty(item.issueId))
                    {
                        foreach (var hora in horasPorTime)
                        {
                            if (item.day < DateTime.Today || item.endTime <= hora.Hora.AddHours(-3))
                            {
                                hora.HorasTotal = hora.HorasTotal.AddMilliseconds(item.duration);
                                if (userNameTMS.Contains(item.user))
                                    hora.HorasTotalTMS = hora.HorasTotalTMS.AddMilliseconds(item.duration);
                                else if (userNameERP.Contains(item.user))
                                    hora.HorasTotalERP = hora.HorasTotalERP.AddMilliseconds(item.duration);
                            }
                            else
                            {
                                hora.HorasTotal = hora.HorasTotal.AddMilliseconds((hora.Hora.AddHours(-3) - item.startTime).TotalMilliseconds);
                                if (userNameTMS.Contains(item.user))
                                    hora.HorasTotalTMS = hora.HorasTotalTMS.AddMilliseconds((hora.Hora.AddHours(-3) - item.startTime).TotalMilliseconds);
                                else if (userNameERP.Contains(item.user))
                                    hora.HorasTotalERP = hora.HorasTotalERP.AddMilliseconds((hora.Hora.AddHours(-3) - item.startTime).TotalMilliseconds);
                            }
                        }
                    }
                }
                int.TryParse(model.TotalAcumuladoPorTime.FirstOrDefault(c => c.Chave == "TMS")?.Valor.ToString(), out int valorAcumuladoTMS);
                int.TryParse(model.TotalAcumuladoPorTime.FirstOrDefault(c => c.Chave == "ERP")?.Valor.ToString(), out int valorAcumuladoERP);

                model.Atualizacoes = horasPorTime.Select(c => new EconometroAtualizacaoViewModel()
                {
                    Valor = Math.Round((c.HorasTotal - new DateTime()).TotalHours * valorHora + valorAcumuladoTMS + valorAcumuladoERP, 2),
                    ValorTMS = Math.Round((c.HorasTotalTMS - new DateTime()).TotalHours * valorHora + valorAcumuladoTMS + valorAcumuladoERP, 2),
                    ValorERP = Math.Round((c.HorasTotalERP - new DateTime()).TotalHours * valorHora + valorAcumuladoTMS + valorAcumuladoERP, 2),
                }).ToList();
            }
            catch (Exception ex)
            {
                var exMessage = ex;
                model.Erro = ex.Message;
                exMessage = ex.InnerException;
                while (exMessage != null)
                {
                    model.Erro = $"{model.Erro} - {exMessage.Message}";
                    exMessage = exMessage.InnerException;
                }
            }
            if (!model.Atualizacoes.Any())
                model.Atualizacoes.Add(new EconometroAtualizacaoViewModel());
            if (DateTime.Now.Hour >= 19 ||
                new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(DateTime.Now.DayOfWeek))
            {
                while (model.Atualizacoes.Count() < 2880)
                    model.Atualizacoes.Add(model.Atualizacoes.Last());
            }

            while (model.Atualizacoes.Count() < 12)
                model.Atualizacoes.Add(model.Atualizacoes.Last());

            // não ficar atualizando a cada 5 segundos
            //model.Atualizacoes.Clear();
            //model.Atualizacoes.Add(new EconometroAtualizacaoViewModel());

            while (model.Atualizacoes.Count() < 12)
                model.Atualizacoes.Add(model.Atualizacoes.Last());

            return model;
        }

        private async Task<TMetricLancamento[]> GetLancamentosTMetricAsync(string tokenTmetric, DateTime data)
        {
            var dataFim = data.AddMonths(1).AddDays(-1);
            var dataInicioStr = data.ToString("yyyy-MM-dd");
            var dataFimStr = (dataFim > DateTime.Today ? dataFim : DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenTmetric);
            var response = await client.GetAsync(@$"https://app.tmetric.com/api/reports/detailed?AccountId=139371&StartDate={dataInicioStr}&EndDate={dataFimStr}");
            if (response != null && response.IsSuccessStatusCode)
            {
                TMetricLancamento[]? metricLancamentos = await response.Content.ReadFromJsonAsync<TMetricLancamento[]>();
                return metricLancamentos ?? Array.Empty<TMetricLancamento>();
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                var erro = response.ReasonPhrase;
                if (!string.IsNullOrEmpty(errorMessage) || !string.IsNullOrEmpty(erro))
                    throw new Exception($"{errorMessage} {erro}".Trim());

            }
            return Array.Empty<TMetricLancamento>();
        }
    }
}
