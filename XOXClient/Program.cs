using System;
using System.Text;

namespace XOXClient
{
    class Program
    {
        public static string GetNameInput()
        {
            Console.WriteLine("Enter desired display name:");
            string name = Console.ReadLine();
            return name;
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Attempting to establish a connection to the server..");
                Connection.Connect(args[0], Convert.ToInt32(args[1]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
