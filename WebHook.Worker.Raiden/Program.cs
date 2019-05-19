using System;
using System.ServiceProcess;
using System.Threading;
using NLog;

namespace WebHook.Worker.Raiden
{
    public static class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private const string WorkerName = "Raiden";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        public static void Main(params string[] args)
        {
            using (var mutex = new Mutex(false, "Global\\" + WorkerName))
            {
                if (!mutex.WaitOne(0, false))
                {
                    Log.Debug("{0} is already running. Only a single instance allowed.", WorkerName);
                    return;
                }

                var arg0 = string.Empty;
                if (args.Length > 0) arg0 = (args[0] ?? string.Empty).ToLower();

                var service = new Raiden();
                if (arg0 == "-console")
                {
                    FakeService(service, args);
                    return;
                }

                var servicesToRun = new ServiceBase[] { new Raiden() };
                ServiceBase.Run(servicesToRun);
            }
        }

        public static void FakeService(Raiden service, params string[] args)
        {
            service.Start(args);
            Console.ReadLine();
        }
    }

    public partial class Raiden
    {
        public void Start(string[] args)
        {
            OnStart(args);
        }
    }
}