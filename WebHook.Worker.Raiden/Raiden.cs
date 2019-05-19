using System;
using System.ServiceProcess;
using NLog;
using Unity;
using WebHook.Common.Lib;
using WebHook.Common.Lib.Contract;
using WebHook.Common.Lib.Enum;
using WebHook.Common.Lib.Model;
using WebHook.Engine.Backbone.Broker;
using WebHook.Engine.Backbone.Contract;
using WebHook.Engine.Configuration;
using WebHook.Engine.Configuration.Contract;

namespace WebHook.Worker.Raiden
{
    public partial class Raiden : ServiceBase
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly IHookMonitor _monitor;

        public Raiden()
        {
            InitializeComponent();

            //DI registration courtesy of unity ioc container
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IHookMonitor, HookMonitor>().
                RegisterType<IConfigurationFactory, ConfigurationFactory>().
                RegisterType<IUtility, Utility>();

            _monitor = container.Resolve<IHookMonitor>();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _monitor.Listen = new Listen
                {
                    RoutingKey = RoutingKey.MTBilling,
                    Queue = RabbitQueue.MTBillingQueue,
                };

                _monitor.Start("Raiden");
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                log.Info("raiden is stopping...");
                _monitor.Stop();
                log.Info("raiden has stopped");
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
            }
        }
    }
}
