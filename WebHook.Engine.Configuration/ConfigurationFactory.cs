using System;
using System.Collections.Generic;
using System.Configuration;
using NLog;
using WebHook.Common.Lib;
using WebHook.Common.Lib.Contract;
using WebHook.Engine.Configuration.Config;
using WebHook.Engine.Configuration.Contract;

namespace WebHook.Engine.Configuration
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        private readonly PipelineEvents events;
        private readonly List<string> moduleCollection;
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public ConfigurationFactory()
        {
            try
            {
                var config = ConfigurationManager.GetSection("WebHook") as WebHookConfigSection;
                if (config == null) throw new NullReferenceException("WebHook  config section cannot be null");

                events = new PipelineEvents();
                moduleCollection = new List<string>();

                log.Info("Searching for modules...");
                var count = 0;

                foreach (ProviderSettings item in config.Modules)
                {
                    var aType = Type.GetType(item.Type);
                    if (aType == null) continue;

                    var module = Activator.CreateInstance(aType) as IPipelineModule;
                    module?.Init(events, item.Parameters);
                    moduleCollection.Add(item.Name);

                    count++;
                    log.Info("Found module {0}", item.Name);
                }

                log.Info("{0} modules installed", count);
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
            }
        }

        public List<string> GetModules()
        {
            return moduleCollection;
        }

        public PipelineEvents GetEvents()
        {
            return events;
        }
    }
}