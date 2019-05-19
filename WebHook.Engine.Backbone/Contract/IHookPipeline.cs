using System.Net.Http;
using System.Threading.Tasks;
using RabbitMQ.Client;
using WebHook.Common.Lib.Model;

namespace WebHook.Engine.Backbone.Contract
{
    public interface IHookPipeline
    {
        IModel RabbitChannel { get; set; }
        HttpClient HttpClient { get; set; }

        Task Invoke(Job job);
    }
}