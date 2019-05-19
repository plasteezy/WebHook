using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client;
using ServiceStack;
using WebHook.Common.Lib.Contract;
using WebHook.Common.Lib.Model;

namespace WebHook.Common.Lib
{
    public class Utility : IUtility
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public async Task FireAsync(string hook, HttpClient httpClient)
        {
            try
            {
                var response = await httpClient.GetAsync(new Uri(hook));
                var body = await response.Content.ReadAsStringAsync();
            }
            catch (WebException ex)
            {
                log.Error(ex.Message);
            }
        }

        public void StartupCheck()
        {
            log.Info("Validating config values...");
            foreach (var item in ConfigurationManager.AppSettings.Keys)
            {
                var key = item.ToString();
                var configValue = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(configValue))
                    log.Warn("Null value provided for config entry with key {0}", key);
                else
                    log.Info("Config value for {0} validated", key);
            }

            log.Info("Config values validation complete");
        }

        public void EnqueueJob(string url, string routingKey)
        {
            var exchange = ConfigurationManager.AppSettings["Rabbit::Exchange"];
            var host = ConfigurationManager.AppSettings["Rabbit::Host"];
            var user = ConfigurationManager.AppSettings["Rabbit::User"];
            var pwd = ConfigurationManager.AppSettings["Rabbit::Pwd"];

            var factory = new ConnectionFactory { HostName = host, UserName = user, Password = pwd };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, "direct");

                    var job = new Job {Url = url};
                    var msgContext = Encoding.UTF8.GetBytes(job.ToJson());

                    channel.BasicPublish(exchange, routingKey, null, msgContext);
                }
            }
        }
    }
}