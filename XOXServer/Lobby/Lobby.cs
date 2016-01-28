using System;
using System.Collections.Generic;
using System.Text;

namespace XOXServer
{
    public class Lobby
    {
        private static List<Connection> _players = new List<Connection>();

        public static int GetPlayersCount
        {
            get
            {
                return _players.Count;
            }
        }

        public static void AddPlayer(Connection conn)
        {
            if (!_players.Contains(conn))
                _players.Add(conn);
        }

        public static Connection FindPlayer(string plrName)
        {
            for (int i = 0; i < _players.Count; ++i)
                if (String.Compare(_players[i].GetName, plrName) == 0)
                    return _players[i];

            return null;
        }

        public static string GetPlayerByName(int index)
        {
            return _players[index].GetName;
        }

        public static void RemovePlayer(Connection conn)
        {
            if (_players.Contains(conn))
                _players.Remove(conn);
        }
    }
}
