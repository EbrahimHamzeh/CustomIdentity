using System.Threading.Tasks;
using DNTCommon.Web.Core;
using Identity.App.Extention;
using Identity.App.Services.Interface;
using Identity.App.Settings;
using Microsoft.Extensions.Options;

namespace Identity.App.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IOptionsSnapshot<AppSettings> _smtpConfig;
        private readonly IWebMailService _webMailService;

        public AuthMessageSender(
            IOptionsSnapshot<AppSettings> smtpConfig,
            IWebMailService webMailService)
        {
            _smtpConfig = smtpConfig;
            _smtpConfig.CheckArgumentIsNull(nameof(_smtpConfig));

            _webMailService = webMailService;
            _webMailService.CheckArgumentIsNull(nameof(webMailService));
        }

        public Task SendEmailAsync<T>(string email, string subject, string viewNameOrPath, T model)
        {
            return _webMailService.SendEmailAsync(
                _smtpConfig.Value.Smtp,
                new[] { new MailAddress { ToName = "", ToAddress = email } },
                subject,
                viewNameOrPath,
                model
            );
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return _webMailService.SendEmailAsync(
                _smtpConfig.Value.Smtp,
                new[] { new MailAddress { ToName = "", ToAddress = email } },
                subject,
                message
            );
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
