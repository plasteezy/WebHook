using System.Collections.Generic;
using WebHook.Common.Lib;

namespace WebHook.Engine.Configuration.Contract
{
    public interface IConfigurationFactory
    {
        List<string> GetModules();

        PipelineEvents GetEvents();
    }
}