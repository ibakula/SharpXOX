﻿using System;
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

        public void HandleReceive(IAsyncResult result)
        {
            
        }
    }
}