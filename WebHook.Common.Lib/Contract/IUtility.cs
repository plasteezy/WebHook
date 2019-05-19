using System.Net.Http;
using System.Threading.Tasks;

namespace WebHook.Common.Lib.Contract
{
    public interface IUtility
    {
        Task FireAsync(string hook, HttpClient httpClient);

        void StartupCheck();

        void EnqueueJob(string url, string routingKey);
    }
}