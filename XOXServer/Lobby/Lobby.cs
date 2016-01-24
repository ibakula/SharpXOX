using System;
using System.Collections.Generic;
using System.Text;

namespace XOXServer
{
    public class Lobby
    {
        private List<Connection> players;

        public string this[int index]
        {
            get
            {
                return players[index].GetName;
            }
        }

        public int GetPlayersCount
        {
            get
            {
                return players.Count;
            }
        }
        
        public void AddPlayer(Connection conn)
        {
            if (!players.Contains(conn))
            {
                players.Add(conn);
            }
        }

        public void RemovePlayer(Connection conn)
        {
            if(players.Contains(conn))
            {
                players.Remove(conn);
            }
        }
    }
}
