using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace XOXServer
{
    public class Server
    {
        public static ManualResetEvent Mre = new ManualResetEvent(false);

        public static void StartListening(string bindServer, int listenPort)
        {
            OpcodesHandler.Init();
            IPHostEntry iphe = Dns.GetHostEntry(bindServer);
            IPEndPoint ipe = new IPEndPoint(iphe.AddressList[0], listenPort);

            Socket listener = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ipe);
                listener.Listen(listenPort);
                Console.WriteLine("Accepting connections on {0}:{1}", ipe.ToString(), listenPort);
                while (true)
                {
                    Mre.Reset();
                    listener.BeginAccept(new AsyncCallback(Connection.Open), listener);
                    Mre.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Issue occured: {0}", e.Message);
            }
            Console.WriteLine("Press any key to finish...");
            Console.ReadKey();
        }
    }
}
