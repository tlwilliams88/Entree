using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Windows.AccessService {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { 
                new AccessService() 
            };

            if (Environment.UserInteractive) {
                RunInteractive(ServicesToRun);
            } else {
                ServiceBase.Run(ServicesToRun);
            }
        }

        static void RunInteractive(ServiceBase[] servicesToRun) {
            Console.WriteLine("Running in interactive mode");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine();

            System.Reflection.MethodInfo onStart = typeof(ServiceBase).GetMethod("OnStart", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            foreach (ServiceBase service in servicesToRun) {
                Console.WriteLine("Starting service {0}...", service.ServiceName);
                onStart.Invoke(service, new object[] { new string[] { } });
            }

            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Press any key to end the processes");
            Console.ReadKey();

            System.Reflection.MethodInfo onStop = typeof(ServiceBase).GetMethod("OnStop", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            foreach (ServiceBase service in servicesToRun) {
                Console.WriteLine("Stopping service {0}...", service.ServiceName);
                onStop.Invoke(service, null);
            }

            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Everything stopped");
        }
    }
}
