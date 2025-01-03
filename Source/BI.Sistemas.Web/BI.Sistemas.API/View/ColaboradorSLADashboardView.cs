using BI.Sistemas.Context;

namespace BI.Sistemas.API.View
{
    public class ColaboradorSLADashboardView
    {
        public int ForaPrazo { get; set; }
        public int DentroPrazo { get; set; }
        public int Pessoal { get; set; }
        public int Setorial { get; set; }
        public int Sistemas { get; set; }
        public HE? HE { get; set; }
        public int FechadosPessoal { get; set; }
        public int FechadosEquipe { get; set; }
        public int FechadosSistemas { get; set; }
        public int Aguardando { get; set; }

        public string FotoTime { get; set; }
        public string FotoColaborador { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cargo { get; set; }
        public string Time { get; set; }

        public int Chamados_SLA_Individual { get; set; }
        public double SLA_Individual { get; set; }
        public double SLA_Time { get; set; }
        public double SLA_Sistemas { get; set; }

        public List<SLAView> TopSLA { get; set; }
        public List<ChamadoView> TabelaForaPrazo { get; set; }
        public List<ChamadoView> TabelaPendentes { get; set; }
        public List<ServicoView> Servicos { get; set; }
        public int[] EvolucaoChamadosAbertos { get; set; }
        public int[] EvolucaoChamadosFechados { get; set; }

        public string LeadTime { get; set; }
        public string LeadTimeEquipe { get; set; }
        public string LeadTimeSistemas { get; set; }
        public bool Desenvolvedor { get; private set; } = false;

        public class SLAView
        {
            public string Nome { get; set; }
            public string Foto { get; set; }
            public double Percentual { get; set; }
            public int DentroDoPrazo { get; set; }

        }
        public class EvolucaoSLAView
        {
            public string Data { get; set; }
            public int Valor { get; set; }
            public string Tipo { get; set; }
        }
        public class ApropriacaoView
        {
            public string Tipo { get; set; }
            public double Valor { get; set; }
            public string Color
            {
                get
                {
                    if (Tipo == "Gestão Volume")
                    {
                        return "#00b050";
                    }
                    else if (Tipo == "SMP")
                    {
                        return "#ff0000";
                    }
                    else if (Tipo == "Nota Fiscal")
                    {
                        return "#e49edd";
                    }
                    else if (Tipo == "Tabela de Preço")
                    {
                        return "#00b0f0";
                    }
                    else if (Tipo == "CTE")
                    {
                        return "#782170";
                    }
                    return string.Empty;
                }
            }
        }
        public class ChamadoView
        {
            public int? Numero { get; set; }
            public string Solicitante { get; set; }
            public string Assunto { get; set; }
            public string? DataAbertura { get; set; }
            public string? DataFechamento { get; set; }
            public string Servico { get; set; }

        }
        public class ServicoView
        {
            public string Servico { get; set; }
            public int Quantidade { get; set; }

        }
    }
}
