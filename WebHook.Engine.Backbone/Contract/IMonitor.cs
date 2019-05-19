using System.Threading.Tasks;
using WebHook.Common.Lib.Model;

namespace WebHook.Engine.Backbone.Contract
{
    public interface IMonitor
    {
        Listen Listen { get; set; }

        Task InitSprint();

        void Start(string consumer);

        void Stop();

        Task SubscribeToChannel(Listen listen);
    }
}