namespace BI.Sistemas.API.View
{
    public class EconometroViewModel
    {
        public decimal ValorAnoAnterior { get; set; }
        public decimal Meta { get; set; }
        public decimal PercentualAumento
        {
            get
            {
                if (ValorAnoAnterior > 0)
                    return Math.Truncate(Meta / ValorAnoAnterior * 100 - 100);
                
                return 0;
            }
        }
        public decimal ValorTotalAcumulado
        {
            get
            {
                return TotalAcumulado.Sum(c => c.Valor);
            }
        }
        public decimal ValorTotalAcumuladoTMS
        {
            get
            {
                return TotalAcumuladoPorTime.Where(c => c.Chave == "TMS").Sum(c => c.Valor);
            }
        }

        public decimal ValorTotalAcumuladoERP
        {
            get
            {
                return TotalAcumuladoPorTime.Where(c => c.Chave == "ERP").Sum(c => c.Valor);
            }
        }
        public string? Erro { get; set; }
        public decimal Estimativa
        {
            get
            {

                if (TotalAcumulado.Any())
                {
                    var timeStanp = DateTime.Today - new DateTime(DateTime.Today.Year, 1, 1);
                    var media = TotalAcumulado.Sum(c => c.Valor) / (decimal)timeStanp.TotalDays;
                    return media * 365;
                }
                return 0;
            }
        }

        public decimal PercentualEstimativa
        {
            get
            {
                if (Meta > 0)
                {
                    return Math.Truncate(Estimativa / Meta * 100 - 100);
                }
                return 0;
            }
        }
        public ICollection<EconometroValorViewModel> TotalAcumulado { get; set; } = new List<EconometroValorViewModel>();
        public ICollection<EconometroValorViewModel> TotalAcumuladoPorTime { get; set; } = new List<EconometroValorViewModel>();
        public ICollection<EconometroValorViewModel> TotalAreas { get; set; } = new List<EconometroValorViewModel>();
        public ICollection<EconometroAtualizacaoViewModel> Atualizacoes { get; set; } = new List<EconometroAtualizacaoViewModel>();
    }

    public class EconometroAtualizacaoViewModel
    {
        public double Valor { get; set; } = 0;
        public double ValorTMS { get; set; } = 0;
        public double ValorERP { get; set; } = 0;
    }

    public class EconometroValorViewModel
    {
        public EconometroValorViewModel(string chave, decimal valor)
        {
            Chave = chave;
            Valor = valor;
        }
        public string Chave { get; set; }
        public decimal Valor { get; set; }
    }
}
