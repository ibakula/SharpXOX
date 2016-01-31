using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace XOXClient
{
    class Connection
    {
        private static Socket _socket;
        private static Queue<Packet> _packets = new Queue<Packet>();
        private static ManualResetEvent _mre = new ManualResetEvent(false);
        private static string _name;

        public static void Connect(string host, int port)
        {
            IPEndPoint ipep = new IPEndPoint(Dns.GetHostAddresses(host)[0], port);
            Socket socket = new Socket(ipep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(ipep, new AsyncCallback(HandleConnect), socket);
            _mre.WaitOne();
        }

        public static void HandleConnect(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndConnect(result);
            _socket = socket;
            _name = Program.GetNameInput();
            Packet packet = new Packet(Opcodes.JOIN, false);
            packet.Write(_name);
            SendWrapper(packet);
            packet = new Packet();
            _socket.BeginReceive(packet.GetData, 0, 53, SocketFlags.None, new AsyncCallback(HandleReceive), packet);
        }

        public static void HandleJoinOpcode(Packet packet)
        {
            string names = String.Empty;
            packet.Read(ref names);
            Console.WriteLine(names);
            Console.WriteLine("Name taken, try not to enter ones from the above.");
            string name = Program.GetNameInput();
            Packet packetToSend = new Packet(Opcodes.JOIN, false);
            packetToSend.Write(name);
            SendWrapper(packetToSend);
        }

        public static void HandleLobbyOpcode(Packet packet)
        {
            if (Game.GameInProgress())
            {
                int index = 0;
                byte field = 0;
                packet.Read(ref index);
                packet.Read(ref field);
                Game.UpdateFields(field, index);
                Console.WriteLine(Game.DrawTable());
                Console.WriteLine("You have lost the match.");
                Game.Reset();
            }
            Console.WriteLine("You are now viewing the lobby.");
            string names = String.Empty;
            packet.Read(ref names);
            Console.WriteLine(names);
            string name = Program.GetNameInput();
            Packet packetToSend = new Packet(Opcodes.START, false);
            packetToSend.Write(name);
            SendWrapper(packetToSend);
        }

        public static void HandleNullOpcode(Packet packet)
        {

        }

        public static void HandleStartOpcode(Packet packet)
        {
            Console.WriteLine("You're now waiting for opponent's turn.");
        }

        public static void HandleTurnOpcode(Packet packet)
        {
            int opponentIndex = 0;
            byte opponentField = 0;
            packet.Read(ref opponentIndex);
            packet.Read(ref opponentField);
            int plrIndex = Game.GetMyIndex(opponentIndex);
            Game.UpdateFields(opponentField, opponentIndex);
            Console.WriteLine(Game.DrawTable());
            Packet packetToSend = new Packet(Opcodes.TURN, false);
            byte field = Game.DoMovement(plrIndex);
            packetToSend.Write(field);

            if (Game.FindWinner() == plrIndex)
            {
                Console.WriteLine("You have won the match, congratulations!");
                Game.Reset();
            }

            SendWrapper(packetToSend);
        }

        public static void HandleReceive(IAsyncResult result)
        {
            Packet packet = (Packet)result.AsyncState;
            int packetSize = packet.GetPacketSize();
            packet.Prepare(packetSize);
            _socket.BeginReceive(packet.GetData, 53, packetSize, SocketFlags.None, new AsyncCallback(HandleReceiveMore), packet);
        }

        public static void HandleReceiveMore(IAsyncResult result)
        {
            Packet packet = (Packet)result.AsyncState;
            _socket.EndReceive(result);
            byte opcode = packet.GetOpcode();

            if (OpcodesHandler.handlerTable[opcode].Handler != null)
                OpcodesHandler.handlerTable[opcode].Handler(packet);

            packet = new Packet();
            _socket.BeginReceive(packet.GetData, 0, 53, SocketFlags.None, new AsyncCallback(HandleReceive), packet);
        }

        public static void SendNext()
        {
            SocketError err;
            _socket.Send(_packets.Peek().GetData, 0, _packets.Peek().GetPacketSize()+53, SocketFlags.None, out err);

            if (err != SocketError.Success)
                Console.WriteLine("Failed to deliver a packet: {0}", err.ToString());
            
            _packets.Dequeue();

            if (_packets.Count != 0)
                SendNext();
        }

        public static void SendWrapper(Packet packet)
        {
            packet.Settle();
            _packets.Enqueue(packet);
            if (_packets.Count == 1)
                SendNext();
        }
    }
}
