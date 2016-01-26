using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOXServer
{
    delegate void FnHandler(Connection client, Packet packet);

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
        public static OpcodesHandler[] handlerTable = new OpcodesHandler[5];
        public static void Init()
        {
            handlerTable[0].Name = "NULL";
            handlerTable[0].handler += delegate(Connection client, Packet pckt) { client.HandleNullOpcode(pckt); };
            handlerTable[1].Name = "JOIN";
            handlerTable[1].handler += delegate(Connection client, Packet pckt) { client.HandleJoinOpcode(pckt); };
            handlerTable[2].Name = "LOBBY";
            handlerTable[2].handler += delegate(Connection client, Packet pckt) { client.HandleLobbyOpcode(pckt); };
            handlerTable[3].Name = "START";
            handlerTable[3].handler += delegate(Connection client, Packet pckt) { client.HandleStartOpcode(pckt); };
            handlerTable[4].Name = "TURN";
            handlerTable[4].handler += delegate(Connection client, Packet pckt) { client.HandleTurnOpcode(pckt); };
        }
        public string Name;
        public FnHandler handler = null;
    }
}
