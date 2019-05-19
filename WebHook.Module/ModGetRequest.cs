using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using NLog;
using WebHook.Common.Lib;
using WebHook.Common.Lib.Contract;
using WebHook.Common.Lib.Enum;

namespace WebHook.Module
{
    public class ModGetRequest
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly IUtility utility;

        public ModGetRequest() : this(new Utility())
        {
        }

        public ModGetRequest(IUtility utility)
        {
            this.utility = utility;
        }

        public void Init(PipelineEvents events, NameValueCollection configElement)
        {
            events.OnFireUrl += OnFireUrl;
        }

        private async Task OnFireUrl(PipelineEventArgs e)
        {
            try
            {
                await utility.FireAsync(e.Job.Url, e.HttpClient);

                e.PipelineResponse = PipelineResponse.Continue;
                e.Transition = Transition.Processed;
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                e.Response = ex.Message;
                e.PipelineResponse = PipelineResponse.Error;

                throw new ApplicationException(ex.Message);
            }
        }
    }
}
