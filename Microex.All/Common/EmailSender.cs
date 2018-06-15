using System;
using System.Threading.Tasks;

namespace Microex.All.Common
{
    public class EmailSender:IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string content)
        {
            throw new NotImplementedException();
        }
    }
}
