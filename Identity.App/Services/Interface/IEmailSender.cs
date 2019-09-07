using System;
using System.Threading.Tasks;

namespace Identity.App.Services.Interface
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync<T>(string email, string subject, string viewNameOrPath, T model);
    }
}
