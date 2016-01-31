using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOXServer
{
    public delegate void FnHandler(Connection client, Packet packet);

    public enum Opcodes : byte
    {
        NULL    = 0,
        JOIN    = 1,
        LOBBY   = 2,
        START   = 3,
        TURN    = 4
    }

    public struct OpcodesHandler
    {
        public OpcodesHandler(Opcodes opcode, FnHandler handler)
        {
            Name = opcode.ToString();
            Handler = null;
            Handler += handler;
        }

        public static OpcodesHandler[] handlerTable = 
        { 
            new OpcodesHandler(Opcodes.NULL, delegate(Connection client, Packet pckt) { client.HandleNullOpcode(pckt); }),
            new OpcodesHandler(Opcodes.JOIN, delegate(Connection client, Packet pckt) { client.HandleJoinOpcode(pckt); }),
            new OpcodesHandler(Opcodes.LOBBY, delegate(Connection client, Packet pckt) { client.HandleLobbyOpcode(pckt); }),
            new OpcodesHandler(Opcodes.START, delegate(Connection client, Packet pckt) { client.HandleStartOpcode(pckt); }),
            new OpcodesHandler(Opcodes.TURN, delegate(Connection client, Packet pckt) { client.HandleTurnOpcode(pckt); })
        };

        public string Name;
        public FnHandler Handler;
    }
}
