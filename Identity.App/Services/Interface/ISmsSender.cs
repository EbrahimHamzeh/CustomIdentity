using System.Threading.Tasks;

namespace Identity.App.Services.Interface
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);

    }
}
