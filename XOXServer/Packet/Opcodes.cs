using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOXServer
{
    public delegate void FnHandler(Connection client, Packet packet);

    enum Opcodes : int
    {
        NULL = 0,
        JOIN,
        LOBBY,
        START,
        TURN
    }

    public struct OpcodesHandler
    {
        public OpcodesHandler(string name, FnHandler handler)
        {
            Name = name;
            Handler = handler;
        }

        public static OpcodesHandler[] handlerTable = 
        { 
            new OpcodesHandler("NULL", delegate(Connection client, Packet pckt) { client.HandleNullOpcode(pckt); }),
            new OpcodesHandler("JOIN", delegate(Connection client, Packet pckt) { client.HandleJoinOpcode(pckt); }),
            new OpcodesHandler("LOBBY", delegate(Connection client, Packet pckt) { client.HandleLobbyOpcode(pckt); }),
            new OpcodesHandler("START", delegate(Connection client, Packet pckt) { client.HandleStartOpcode(pckt); }),
            new OpcodesHandler("TURN", delegate(Connection client, Packet pckt) { client.HandleTurnOpcode(pckt); })
        };

        public string Name;
        public FnHandler Handler;
    }
}
