using BI.Sistemas.API.View;

namespace BI.Sistemas.API.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EnviarEmailDados dados);
    }
}
