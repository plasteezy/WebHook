namespace WebHook.Common.Lib
{
    public class PipelineEvents
    {
        public PipelineDelegate<PipelineEventArgs> OnFireUrl { get; set; }
        public PipelineDelegate<PipelineEventArgs> OnErrorOccurred { get; set; }
    }
}