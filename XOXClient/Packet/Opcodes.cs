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
        public static Action<Packet>[] Handler = 
        { 
            (Packet pckt) => Connection.HandleNullOpcode(pckt), 
            (Packet pckt) => Connection.HandleJoinOpcode(pckt), 
            (Packet pckt) => Connection.HandleLobbyOpcode(pckt), 
            (Packet pckt) => Connection.HandleStartOpcode(pckt), 
            (Packet pckt) => Connection.HandleTurnOpcode(pckt) 
        };
    }
}
