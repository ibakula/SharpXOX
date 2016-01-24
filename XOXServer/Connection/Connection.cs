using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace XOXServer
{
    public class Connection
    {
        private Connection(Socket client)
        {
            _client = null;
        }

        private Socket _client;
        private Queue<Packet> _packets;
        protected string _name;

        public void HandleJoinOpcode(Packet packet)
        {

        }

        public void HandleLobbyOpcode(Packet packet)
        {

        }

        public void HandleNullOpcode(Packet packet)
        {

        }

        public void HandleStartOpcode(Packet packet)
        {

        }

        public void HandleTurnOpcode(Packet packet)
        {

        }

        public void HandleReceive(IAsyncResult result)
        {
            Packet packet = (Packet)result.AsyncState;
            _client.BeginReceive(packet.GetData, 3, packet.GetPacketSize(), SocketFlags.None, new AsyncCallback(HandleReceiveMore), packet);
        }

        public void HandleReceiveMore(IAsyncResult result)
        {
            Packet packet = (Packet)result.AsyncState;

            if (OpcodesHandler.handlerTable[packet.GetOpcode()].handler != null)
                OpcodesHandler.handlerTable[packet.GetOpcode()].handler(this, packet);

            packet = new Packet();
            _client.BeginReceive(packet.GetData, 0, 3, SocketFlags.None, new AsyncCallback(HandleReceive), packet);
        }

        public static void Open(IAsyncResult result)
        {
            Server.Mre.Set();
            Socket listener = (Socket)result;
            Socket handler = listener.EndAccept(result);
            Connection connection = new Connection(handler);
            Server.SLobby.AddPlayer(connection);
            Packet packet = new Packet();
            handler.BeginReceive(packet.GetData, 0, 3, SocketFlags.None, new AsyncCallback(connection.HandleReceive), packet);
        }
    }
}
