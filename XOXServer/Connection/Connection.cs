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
        private string _name;
        private Game _match;

        public string GetName
        {
            get
            {
                return _name;
            }
        }

        public void HandleJoinOpcode(Packet packet)
        {
            string Name = packet.Read();
            Lobby.AddPlayer(this);
        }

        public void HandleLobbyOpcode(Packet packet)
        {
            string Names = String.Empty;
            for (int i = 0; i < Lobby.GetPlayersCount; ++i)
                Names += Lobby.GetPlayerByName(i);

            Packet packetToSend = new Packet(Convert.ToInt32(Opcodes.LOBBY));
            packetToSend.Write<string>(Names);
            packetToSend.Finalize();
            SendWrapper(packetToSend);
        }

        public void HandleNullOpcode(Packet packet)
        {
            
        }

        public void HandleStartOpcode(Packet packet)
        {
            if (_match != null) // In a match already?
                return;

            string opponentName = packet.Read();
            Connection opponent = Lobby.FindPlayer(opponentName);
            Packet packetToSend;
            if (opponent == null) // Misspelled?
            {
                packetToSend = new Packet(Convert.ToInt32(Opcodes.NULL));
                packetToSend.Finalize();
                SendWrapper(packetToSend);
                return;
            }

            _match = new Game(this, opponent);
            opponent._match = _match;
            Lobby.RemovePlayer(this);
            Lobby.RemovePlayer(opponent);
            packet = new Packet(Convert.ToInt32(Opcodes.TURN)); // Take turn
            packet.Write(_match.GetTableData);
            packet.Finalize();
            SendWrapper(packet);
            packetToSend = new Packet(Convert.ToInt32(Opcodes.START)); // Signal wait
            packetToSend.Finalize();
            opponent.SendWrapper(packetToSend);
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
            Lobby.AddPlayer(connection);
            Packet packet = new Packet();
            handler.BeginReceive(packet.GetData, 0, 3, SocketFlags.None, new AsyncCallback(connection.HandleReceive), packet);
        }

        private void SendWrapper(Packet packet)
        {
            SocketError err;
            _client.Send(packet.GetData, 0, packet.GetPacketSize(), SocketFlags.None, out err);
            if (err != SocketError.Success)
                Console.WriteLine("Client \"{0}\" Send Code Error: SocketError Num {1}", _name, err.ToString());
        }
    }
}
