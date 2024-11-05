using BI.Sistemas.API.Repository;
using BI.Sistemas.API.View;
using BI.Sistemas.Domain;

namespace BI.Sistemas.API.Service
{
    public class EmailSLAService
    {
        private readonly IColaboradorRepository _colaboradorRepository;
        public EmailSLAService(IColaboradorRepository colaboradorRepository)
        {
            _colaboradorRepository = colaboradorRepository;
        }

        public Task SendEmailAsync(EnviarEmailDados dados)
        {
            ColaboradorDashboardView colaborador = new ColaboradorDashboardView();

            var pessoa = _colaboradorRepository.GetPessoa(dados.Id);
            if (pessoa == null)
                throw new Exception("Arquivo não localizado ou duplicado");

            try
            {
                Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)outlookApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

                if (dados.Oficial)
                {
                    var coordenador = pessoa.Time == "TMS" ? "tulio@patrus.com.br" : "marco.barros@patrus.com.br";
                    mailItem.CC = $"rogerio.simoes@patrus.com.br;{coordenador}";
                    mailItem.To = pessoa.Email;
                }
                else
                {
                    mailItem.To = "vitor.fernandessouza@patrus.com.br";
                }

                var tabelaAtividade = @"vitor";
                string gif = string.Empty;
                string mensagem = string.Empty;
                var engajamentoDevOps = dados.DevOps == 0 ? "0,0" : string.Format("{0:#.#,##}", Math.Round(dados.DevOps, 1));
                if (dados.Engajamento > 100)
                {
                    gif = Gifs.ListasGifs(NivelGif.Cinquenta);
                    mensagem = $@"Seu engajamento foi de <b>{dados.Engajamento}% e lançamentos pelo Devops de {engajamentoDevOps}%. Você registrou um tempo de trabalho superior em relação ao ponto.</b> Atenção com os lançamentos.";
                }
                else if (dados.Engajamento >= 90 && dados.DevOps >= 95)
                {
                    mensagem = $"Seus lançamentos estão excelentes, <b>parabéns</b>✅. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                    gif = Gifs.ListasGifs(NivelGif.Noventa);
                }
                else if (dados.Engajamento >= 0 && dados.DevOps >= 95)
                {
                    mensagem = $"<b>Atenção com os lançamentos no dia à dia</b>⚠️. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                    gif = Gifs.ListasGifs(NivelGif.Cinquenta);
                }
                else if (dados.Engajamento >= 90 && dados.DevOps < 95)
                {
                    mensagem = $"<b>Atenção com os lançamentos dentro das atividades do Devops</b>⚠️. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                    gif = Gifs.ListasGifs(NivelGif.Cinquenta);
                }
                else if (dados.Engajamento < 90 && dados.DevOps < 95)
                {
                    mensagem = $"<b>Seus lançamentos não estão legais</b>, mais atenção com os lançamentos do dia à dia e dentro das atividades do Devops ❌. <br> Seu engajamento foi de <b>{dados.Engajamento}%</b> e seu lançamento pelo Devops <b>{engajamentoDevOps}%</b>.";
                    gif = Gifs.ListasGifs(NivelGif.VinteCinco);
                }

                mailItem.Subject = $@"[SISTEMAS] APURAÇÃO DE HORAS SEMANAL - {pessoa.Nome.ToUpper()} ";
                mailItem.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
                string email = DateTime.Now.Hour < 12 ? "Bom dia" : "Boa tarde";

                string msgHTMLBody = $@"
             <html>vitor";
                mailItem.HTMLBody = msgHTMLBody;
                mailItem.Send();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}
