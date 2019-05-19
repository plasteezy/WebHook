using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client;
using ServiceStack;
using WebHook.Common.Lib.Contract;
using WebHook.Common.Lib.Model;
using WebHook.Engine.Backbone.Contract;
using WebHook.Engine.Backbone.Pipeline;
using WebHook.Engine.Configuration.Contract;

namespace WebHook.Engine.Backbone.Broker
{
    public class HookMonitor : IHookMonitor
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly IUtility utility;

        public Thread SprintThread { get; set; }

        protected internal IHookPipeline HookPipeline;
        private readonly IConfigurationFactory configurationFactory;

        public HookMonitor(IConfigurationFactory configurationFactory, IUtility utility)
        {
            this.configurationFactory = configurationFactory;
            this.utility = utility;
        }

        public Listen Listen { get; set; }

        public void Start(string consumer)
        {
            log.Info("hook monitor v0.5");
            log.Info("worker: {0}", consumer);
            log.Info("hook monitor is booting");
            log.Info("running startup diagnosis");

            utility.StartupCheck();

            log.Info("start up diagnosis complete");

            SprintThread = new Thread(async () => await InitSprint()) { Name = consumer + "-Thread" };
            SprintThread.Start();
        }

        public void Stop()
        {
            SprintThread.Join(TimeSpan.FromSeconds(10.0));
        }

        public async Task InitSprint()
        {
            try
            {
                log.Info("initializing hook monitor...");

                HookPipeline = new HookPipeline(configurationFactory);
                await SubscribeToChannel(Listen);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public async Task SubscribeToChannel(Listen listen)
        {
            log.Info("subscribing to rabbit mq channel");

            var exchange = ConfigurationManager.AppSettings["Rabbit::Exchange"];
            var host = ConfigurationManager.AppSettings["Rabbit::Host"];
            var user = ConfigurationManager.AppSettings["Rabbit::User"];
            var pwd = ConfigurationManager.AppSettings["Rabbit::Pwd"];

            var factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pwd,
                AutomaticRecoveryEnabled = true
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, "direct");
                    var qName = channel.QueueDeclare(listen.Queue.ToString(), true, false, false, null);

                    channel.QueueBind(qName, exchange, listen.RoutingKey.ToString());

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(qName, true, consumer);

                    HookPipeline.RabbitChannel = channel;
                    HookPipeline.HttpClient = new HttpClient();

                    log.Info("subscription to rabbit mq channel complete");
                    log.Info("initialization complete. billing is up and running");

                    while (true)
                    {
                        log.Debug("waiting for a job");
                        var obj = consumer.Queue.Dequeue();

                        if (obj == null) return;
                        var body = obj.Body;

                        var job = Encoding.UTF8.GetString(body).FromJson<Job>();
                        await HookPipeline.Invoke(job);
                    }
                }
            }
        }
    }
}