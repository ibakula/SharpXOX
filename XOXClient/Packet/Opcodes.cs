using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOXClient
{
    public delegate void FnHandler(Packet packet);

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
            Handler = handler;
        }

        public static OpcodesHandler[] handlerTable = 
        { 
            new OpcodesHandler(Opcodes.NULL, delegate(Packet pckt) { Connection.HandleNullOpcode(pckt); }),
            new OpcodesHandler(Opcodes.JOIN, delegate(Packet pckt) { Connection.HandleJoinOpcode(pckt); }),
            new OpcodesHandler(Opcodes.LOBBY, delegate(Packet pckt) { Connection.HandleLobbyOpcode(pckt); }),
            new OpcodesHandler(Opcodes.START, delegate(Packet pckt) { Connection.HandleStartOpcode(pckt); }),
            new OpcodesHandler(Opcodes.TURN, delegate(Packet pckt) { Connection.HandleTurnOpcode(pckt); })
        };

        public string Name;
        public FnHandler Handler;
    }
}
