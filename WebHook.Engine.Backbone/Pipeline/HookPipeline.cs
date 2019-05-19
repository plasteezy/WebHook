using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client;
using WebHook.Common.Lib;
using WebHook.Common.Lib.Model;
using WebHook.Engine.Backbone.Contract;
using WebHook.Engine.Configuration.Contract;

namespace WebHook.Engine.Backbone.Pipeline
{
    public class HookPipeline : IHookPipeline
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly List<string> moduleCollection;
        private readonly PipelineEvents events;
        public HttpClient HttpClient { get; set; }
        public IModel RabbitChannel { get; set; }

        public HookPipeline(IConfigurationFactory configurationFactory)
        {
            events = configurationFactory.GetEvents();
            moduleCollection = configurationFactory.GetModules();
        }

        public async Task Invoke(Job job)
        {
            var args = new PipelineEventArgs(job, events, moduleCollection, HttpClient, RabbitChannel);
            var errorModuleExists = false;

            try
            {
                log.Debug("job received: {0}", job.Url);

                if (events.OnFireUrl == null) return;
                if (args.Cancel) throw new ApplicationException(args.Response);
                await events.OnFireUrl(args);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (events.OnErrorOccurred != null) errorModuleExists = true;
            }

            if (errorModuleExists) await events.OnErrorOccurred(args);
        }
    }
}