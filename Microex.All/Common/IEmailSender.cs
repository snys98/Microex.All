using System.Threading.Tasks;

namespace Microex.All.Common
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string content);
    }
}
