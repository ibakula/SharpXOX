using System;

namespace XOXServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("IP and port were not provided.");
                Console.ReadKey();
                return;
            }
            Server.StartListening(args[0], Int32.Parse(args[2]));
        }
    }
}
