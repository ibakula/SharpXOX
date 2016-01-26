using System;
using System.Collections.Generic;
using System.Text;

namespace XOXServer
{
    public class Lobby
    {
        private static List<Connection> _players;

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
