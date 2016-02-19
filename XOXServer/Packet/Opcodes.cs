using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOXServer
{
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
        public static Action<Connection, Packet>[] Handler = 
        { 
            (Connection client, Packet pckt) => client.HandleNullOpcode(pckt), 
            (Connection client, Packet pckt) => client.HandleJoinOpcode(pckt), 
            (Connection client, Packet pckt) => client.HandleLobbyOpcode(pckt), 
            (Connection client, Packet pckt) => client.HandleStartOpcode(pckt), 
            (Connection client, Packet pckt) => client.HandleTurnOpcode(pckt) 
        };
    }
}
