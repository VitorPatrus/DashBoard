using BI.Sistemas.API.Interfaces;
using BI.Sistemas.API.Repository;
using BI.Sistemas.API.View;
using BI.Sistemas.Context;
using BI.Sistemas.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Outlook;
using System.Net.Mail;
using System.Net;

namespace BI.Sistemas.API.Service
{
    public class EmailService : IEmailService
    {
        private readonly IColaboradorRepository _colaboradorRepository;
        public EmailService(IColaboradorRepository colaboradorRepository)
        {
            _colaboradorRepository = colaboradorRepository;
        }
        public Task SendEmailAsync(EnviarEmailDados dados)
        {
            EnviarEmailDados email = new EnviarEmailDados();

            var pessoa = _colaboradorRepository.GetPessoa(dados.Id);
            if (pessoa == null)
                throw new System.Exception("Arquivo não localizado ou duplicado");

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

                var tabelaAtividade = @"
            <table style='border-collapse: collapse; width: 100%;'>
                <thead>
                    <tr>
                        <th style='border: 1px solid #000; padding: 8px;'>Atividade</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Ticket</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Tipo</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Data</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Horas</th>
                    </tr>
                </thead>
                <tbody>
                    " + string.Join("", dados.Lista.Select(c => $@"
                    <tr>
                        <td style='border: 1px solid #000; padding: 8px;'>{c.Atividade}</td>
                        <td style='border: 1px solid #000; padding: 8px;'>{c.Ticket}</td>
                        <td style='border: 1px solid #000; padding: 8px;'>{c.Tipo}</td>
                        <td style='border: 1px solid #000; padding: 8px;'>{c.Data.Substring(0, 10)}</td>
                        <td style='border: 1px solid #000; padding: 8px;'>{c.Horas.ToString("HH:mm")}</td>
                    </tr>")) + @"
                </tbody>
            </table>";

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

                string msgHTMLBody = $@"
             <html>
                <head></head>
                <body>
                    {email.Apresentacao()}, {pessoa.Nome},<br><br>
                    Segue o dashboard com os lançamentos do período.<b> {dados.Periodo}.</b><br><br>
                    <div style=""min-width:600px!important;margin-left:auto;margin-right:auto; ""></div>
                    <img src=""{gif}"" width=""80"" height=""80""/><br>
                    {mensagem}<br><br>
                    <img align=""baseline"" border=""1"" hspace=""0"" src=""{dados.Foto}"" width=""900"" height=""230"" hold="" /> ""></img><br>
                    <i>
                    <br><br>
                    Segue abaixo a tabela com os lançamentos individuais do TMETRIC:
                    <br><br><hr /><br>
                    <b>Atividades trabalhadas</b><br>
                    <i>Atividades realizadas durante o período {dados.Periodo}</i>
                    <br><br>
                    {tabelaAtividade}
                    <br><br>
                    Caso encontre alguma inconsistência em relação às informações contidas neste e-mail, favor nos contatar.<br><br>
                    Atenciosamente,<br>
                    Vitor Fernandes.
                </body>
            </html>";

                mailItem.HTMLBody = msgHTMLBody;
                mailItem.Send();
                return Task.CompletedTask;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

        public Task SendSLAEmail(EnviarDadosSLA dados)
        {
            return Task.Run(() =>
            {
                var pessoa = _colaboradorRepository.GetPessoa(dados.Id);
                if (pessoa == null)
                    throw new System.Exception("Arquivo não localizado ou duplicado");

                try
                {
                    Application outlookApp = new Application();
                    EnviarEmailDados email = new EnviarEmailDados();
                    NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

                    var tabelaAtividade = @"
            <table style='border-collapse: collapse; width: 100%;'>
                <thead>
                    <tr>
                        <th style='border: 1px solid #000; padding: 8px;'>Número</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Solicitante</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Assunto</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Serviço</th>
                        <th style='border: 1px solid #000; padding: 8px;'>Abertura</th>
                    </tr>
                </thead>
                <tbody>
                    " + string.Join("", dados.ListaForaPrazo.Select(c => $@"
                    <tr>
                        <td style='border: 1px solid #000; padding: 8px;'></td>
                        <td style='border: 1px solid #000; padding: 8px;'></td>
                        <td style='border: 1px solid #000; padding: 8px;'></td>
                        <td style='border: 1px solid #000; padding: 8px;'></td>
                        <td style='border: 1px solid #000; padding: 8px;'></td>
                    </tr>")) + @"
                </tbody>
            </table>";

                    string gif = string.Empty;
                    string mensagem = string.Empty;

                    if (dados.SLA >= 95)
                    {
                        mensagem = $"Seus SLA´s estão excelentes, <b>parabéns</b>✅. Seu SLA foi de <b>{dados.SLA}%</b> .";
                        gif = Gifs.ListasGifs(NivelGif.Noventa);
                    }
                    else if (dados.SLA >= 90)
                    {
                        mensagem = $"<b>Atenção com os SLA´s</b>⚠️. Seu SLA foi de <b>{dados.SLA}%</b> .";
                        gif = Gifs.ListasGifs(NivelGif.Cinquenta);
                    }
                    else if (dados.SLA < 90)
                    {
                        mensagem = $"<b>Seus SLA´s não estão legais</b>, mais atenção com eles ❌. Seu SLA foi de <b>{dados.SLA}%</b> .";
                        gif = Gifs.ListasGifs(NivelGif.VinteCinco);
                    }

                    MailItem mailItem = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);
                    mailItem.Subject = $@"[SUPORTE] APURAÇÃO DE HORAS SEMANAL - {pessoa.Nome.ToUpper()} ";
                    string msgHTMLBody = @$"
                    <html>
                        <head></head>
                        <body>
                            {email.Apresentacao()}, {pessoa.Nome},<br><br>
                            Segue o dashboard com os lançamentos do período.<b> dados.Periodo.</b><br><br>
                            <div style=""min-width:600px!important;margin-left:auto;margin-right:auto;""></div>
                            <img src=""{gif}"" width=""80"" height=""80""/><br>
                            {mensagem}<br><br>
                            <img align=""baseline"" border=""1"" hspace=""0"" src=""{dados.Foto}"" width=""900"" height=""230"" /><br>
                            <i>
                            <br><br>
                            Segue abaixo a tabela com os Chamados Pendentes:
                            <br><br><hr /><br>
                            <b>Atividades Pendentes</b><br>
                            <i>Dados referentes aos dias dados.Periodo</i>
                            <br><br>
                            {tabelaAtividade}
                            <br><br>
                            Caso encontre alguma inconsistência em relação às informações contidas neste e-mail, favor nos contatar.<br><br>
                            Atenciosamente,<br>
                            Vitor Fernandes.
                        </body>
                    </html>";

                    mailItem.HTMLBody = msgHTMLBody;
                    string coordenador = "";
                    string coordenador2 = "";

                    if (dados.Oficial && false)
                    {

                        if (pessoa.Time == "Suporte TMS")
                        {
                            coordenador = "tulio@patrus.com.br";
                            coordenador2 = "paulosouza@patrus.com.br";
                        }
                        else if (pessoa.Time == "Suporte ERP")
                        {
                            coordenador = "marcusethur@patrus.com.br";
                            coordenador2 = "marco.barros@patrus.com.br";
                        }
                        else if (pessoa.Time == "EDI")
                        {
                            coordenador = "natalia@patrus.com.br";
                            coordenador2 = "tulio@patrus.com.br";
                        }
                        else if (pessoa.Time == "CRM")
                        {
                            coordenador = "tulio@patrus.com.br";
                            coordenador2 = "ricardosantos@patrus.com.br";
                        }
                    }
                    else
                    {
                        mailItem.To = "vitor.fernandessouza@patrus.com.br";
                    }

                    //mailItem.CC = $"rogerio.simoes@patrus.com.br;{coordenador}{coordenador2}";
                    //mailItem.To = pessoa.Email;
                    mailItem.Send();

                    return true;
                }
                catch (System.Exception e)
                {
                    throw new System.Exception();
                }
            });
        }
    }
}
