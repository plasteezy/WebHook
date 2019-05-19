using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using RabbitMQ.Client;
using WebHook.Common.Lib.Enum;
using WebHook.Common.Lib.Model;

namespace WebHook.Common.Lib
{
    public class PipelineEventArgs : CancelEventArgs
    {
        public PipelineResponse PipelineResponse;
        public Transition Transition;
        public PipelineEvents Events;
        public HttpClient HttpClient;
        public List<string> ModuleCollection;

        public string Response { get; set; }
        public Job Job { get; set; }
        public IModel RabbitChannel { get; set; }

        public PipelineEventArgs(Job job, PipelineEvents events, List<string> moduleCollection, HttpClient httpClient, IModel rabbitChannel)
        {
            Job = job;
            Events = events;
            ModuleCollection = moduleCollection;
            HttpClient = httpClient;
            RabbitChannel = rabbitChannel;
            Response = string.Empty;
            PipelineResponse = PipelineResponse.Ignore;
            Transition = Transition.New;
        }
    }
}