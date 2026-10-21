using System;
using System.Linq;
using System.Threading;
using GalaxyCoreServer;

namespace SimpleMmoServer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            int port = 30200;
            int portIndex = Array.IndexOf(args, "--port");
            if (portIndex >= 0 && (portIndex + 1 >= args.Length ||
                !int.TryParse(args[portIndex + 1], out port) || port < 1 || port > 65535))
                throw new ArgumentException("--port requires a UDP port between 1 and 65535.");

            using var shutdown = new ManualResetEventSlim();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                shutdown.Set();
            };
            AppDomain.CurrentDomain.ProcessExit += (_, _) => shutdown.Set();

            try
            {
                _ = new Server(port);
                Console.WriteLine($"SimpleMmoServer is listening on UDP {port}. Type exit or press Ctrl+C to stop.");
                if (args.Contains("--smoke-test"))
                {
                    // Exercise every room and every migrated physics body locally, without clients.
                    SmokeTest.Run();
                    return;
                }
                if (Console.IsInputRedirected)
                {
                    // A service/container often has closed stdin; do not spin on ReadLine returning null.
                    shutdown.Wait();
                }
                else
                {
                    while (!shutdown.IsSet)
                    {
                        var command = Console.ReadLine();
                        if (command == null || command.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                    }
                }
            }
            finally
            {
                GalaxyCore.Stop();
            }
        }
    }
}
