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
            _name = String.Empty;
        }

        private Socket _client;
        private Queue<Packet> _packets = new Queue<Packet>();
        private string _name;
        private Game _match;

        public string GetName
        {
            get
            {
                return _name;
            }
        }

        // ToDo: We should update everyone with the new lobby list on each new join.
        public void HandleJoinOpcode(Packet packet)
        {
            string name = String.Empty;
            packet.Read(ref name);
            if (Lobby.FindPlayer(name) != null)
            {
                Packet packetToSend = new Packet(Opcodes.JOIN, false);
                string names = String.Empty;
                for (int i = 0; i < Lobby.GetPlayersCount; ++i)
                    names += "\n" + Lobby.GetPlayerByName(i);

                packetToSend.Write(names);
                SendWrapper(packetToSend);
                return;
            }
            Lobby.AddPlayer(this);
            _name = name;
            HandleLobbyOpcode(packet);
        }

        public void HandleLobbyOpcode(Packet packet)
        {
            string names = String.Empty;
            for (int i = 0; i < Lobby.GetPlayersCount; ++i)
                names += "\n" + Lobby.GetPlayerByName(i);

            Packet packetToSend = new Packet(Opcodes.LOBBY, false);
            if (_match != null)
            {
                int index = _match.GetMyIndex(_match.GetOponnent(this));
                packetToSend.Write(index);
                byte field = 0;
                packet.Read(ref field);
                packetToSend.Write(field);
            }
            packetToSend.Write(names);
            SendWrapper(packetToSend);
        }

        public void HandleNullOpcode(Packet packet)
        {
            
        }

        public void HandleStartOpcode(Packet packet)
        {
            if (_match != null) // In a match already?
                return;

            string opponentName = String.Empty;
            packet.Read(ref opponentName);
            Connection opponent = Lobby.FindPlayer(opponentName);
            if (opponent == null || opponent == this) // Misspelled?
            {
                HandleLobbyOpcode(packet);
                return;
            }

            _match = new Game(this, opponent);
            opponent._match = _match;
            Lobby.RemovePlayer(this);
            Lobby.RemovePlayer(opponent);
            Packet packetToSend = new Packet(Opcodes.TURN, false); // Take turn
            packetToSend.Write((_match.GetMyIndex(opponent)+1));
            SendWrapper(packetToSend);
            packetToSend = new Packet(Opcodes.START, false); // Signal wait
            opponent.SendWrapper(packetToSend);
        }

        public void HandleTurnOpcode(Packet packet)
        {
            if (_match == null) // Did the other player disconnect?
                return;

            byte field = 0;
            packet.Read(ref field);

            _match.DoMovement(field, this);

            if (_match.FindWinner() == this)
            {
                Packet packetToSend = new Packet(Opcodes.JOIN, false);
                packetToSend.Write(_name);
                packetToSend.Settle();
                // Just moving read position
                packetToSend.GetPacketSize();
                packetToSend.GetOpcode();
                Connection opponent = _match.GetOponnent(this);
                _match = null; // this is a winner, he doesn't require any updated field views.
                HandleJoinOpcode(packetToSend);
                packetToSend = new Packet(Opcodes.JOIN, false);
                packetToSend.Write(opponent.GetName);
                packetToSend.Write(field);
                packetToSend.Settle();
                // Just moving read position
                packetToSend.GetPacketSize();
                packetToSend.GetOpcode();
                opponent.HandleJoinOpcode(packetToSend);
                opponent._match = null; // opponent lost, show him the final move.
            }
            else
            {
                Packet packetToSend = new Packet(Opcodes.START, false); // Signal wait.
                SendWrapper(packetToSend);
                packetToSend = new Packet(Opcodes.TURN, false); // Take turn.
                packetToSend.Write((_match.GetMyIndex(this)+1));
                packetToSend.Write(field);
                _match.GetOponnent(this).SendWrapper(packetToSend);
            }
        }

        public void HandleReceive(IAsyncResult result)
        {
            Packet packet = (Packet)result.AsyncState;
            SocketError err;
            int recvBytes = _client.EndReceive(result, out err);
            if (recvBytes < 53 || err != SocketError.Success)
            {
                EndGameOnDisconnect();
                return;
            }
            int packetSize = packet.GetPacketSize();
            packet.Prepare(packetSize);
            RecvWrapper(packet.GetData, 53, packetSize, SocketFlags.None, new AsyncCallback(HandleReceiveMore), packet);
        }

        public void HandleReceiveMore(IAsyncResult result)
        {
            Packet packet = (Packet)result.AsyncState;
            byte opcode = packet.GetOpcode();

            if (OpcodesHandler.Handler[opcode] != null)
                OpcodesHandler.Handler[opcode](this, packet);

            packet = new Packet();
            RecvWrapper(packet.GetData, 0, 53, SocketFlags.None, new AsyncCallback(HandleReceive), packet);
        }

        public static void Open(IAsyncResult result)
        {
            Server.Mre.Set();
            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);
            Connection connection = new Connection(handler);
            connection._client = handler;
            Lobby.AddPlayer(connection);
            Packet packet = new Packet();
            connection.RecvWrapper(packet.GetData, 0, 53, SocketFlags.None, new AsyncCallback(connection.HandleReceive), packet);
        }

        private void RecvWrapper(byte[] buffer, int offset, int size, SocketFlags flags, AsyncCallback callback, object state)
        {
            SocketError err = SocketError.Success;
            try
            {
                _client.BeginReceive(buffer, offset, size, flags, out err, callback, state);
            }
            catch (Exception /*e*/)
            {
                if (err == SocketError.Success)
                    err = SocketError.SocketError;
            }
            finally
            {
                if (err != SocketError.Success)
                {
                    Console.Error.WriteLine("Client \"{0}\" Recv Code Error: SocketError Num {1}", _name, err.ToString());
                    EndGameOnDisconnect();
                }
            }

        }

        private void SendNext()
        {
            SocketError err = SocketError.Success;
            try
            {
                _client.Send(_packets.Peek().GetData, 0, _packets.Peek().GetPacketSize() + 53, SocketFlags.None, out err);
            }
            catch (Exception /*e*/)
            {
                if (err == SocketError.Success)
                    err = SocketError.SocketError;
            }
            finally
            {
                if (err != SocketError.Success)
                {
                    Console.Error.WriteLine("Client \"{0}\" Send Code Error: SocketError Num {1}", _name, err.ToString());
                    EndGameOnDisconnect();
                }
            }

            _packets.Dequeue();

            if (_packets.Count != 0)
                SendNext();
        }

        private void SendWrapper(Packet packet)
        {
            packet.Settle();
            _packets.Enqueue(packet);
            if (_packets.Count == 1)
                SendNext();
        }

        private void EndGameOnDisconnect()
        {
            if (_match != null)
            {
                Connection opponent = _match.GetOponnent(this);
                Packet packetToSend = new Packet(Opcodes.JOIN, false);
                packetToSend.Write(opponent.GetName);
                packetToSend.Settle();
                // Just moving read position
                packetToSend.GetPacketSize();
                packetToSend.GetOpcode();
                opponent._match = null; // End game, player disconnected.
                _match = null;
                opponent.HandleJoinOpcode(packetToSend);
            }
            else Lobby.RemovePlayer(this);
            Console.Error.WriteLine("Shutting down connection with {0}.", _name);
            try
            {
                _client.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _client.Close();
            }
        }
    }
}
