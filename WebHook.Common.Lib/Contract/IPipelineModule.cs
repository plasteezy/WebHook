using System.Collections.Specialized;

namespace WebHook.Common.Lib.Contract
{
    public interface IPipelineModule
    {
        void Init(PipelineEvents events, NameValueCollection configElement);
    }
}