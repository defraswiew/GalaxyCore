using System;

namespace SimpleMmoServer
{
    class Program
    {
        static Server server;
        static void Main(string[] args)
        {           
            server = new Server();
            Console.ReadLine();
        }
    }
}
