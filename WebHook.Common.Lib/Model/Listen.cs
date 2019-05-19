using WebHook.Common.Lib.Enum;

namespace WebHook.Common.Lib.Model
{
    public class Listen
    {
        public RabbitQueue Queue { get; set; }

        public RoutingKey RoutingKey { get; set; }
    }

    public class Job
    {
        public string Url { get; set; }
    }
}