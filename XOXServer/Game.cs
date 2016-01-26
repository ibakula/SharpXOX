using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOXServer
{
    class Game
    {
        public Game(Connection plrOne, Connection plrTwo)
        {
            _players = new Connection[2] 
            { 
                plrOne, 
                plrTwo 
            };
            _fields = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        private Connection[] _players;
        private byte[] _fields; 

        public byte[] GetTableData
        {
            get
            {
                return _fields;
            }
        }

        public void DoMovement(byte field, Connection plr)
        {
            int whoAmI = (plr == _players[0] ? 0 : (plr == _players[1] ? 1 : -1));
            if (whoAmI < 0 || field > 9 || field < 1 || _fields[field - 1] != 0)
                return;

            _fields[field - 1] = Convert.ToByte(whoAmI);
        }

        public Connection FindWinner()
        {
            Connection Winner = null;

            for (int i = 1; i < 3; ++i)
            {
                if ((_fields[0] == i && _fields[1] == i && _fields[2] == i) ||
                    (_fields[3] == i && _fields[4] == i && _fields[5] == i) ||
                    (_fields[6] == i && _fields[7] == i && _fields[8] == i) ||
                    (_fields[0] == i && _fields[4] == i && _fields[8] == i) ||
                    (_fields[2] == i && _fields[4] == i && _fields[6] == i))
                {
                    Winner = _players[i - 1];
                    break;
                }
            }
            return Winner;
        }

        public Connection GetPlayer(int index)
        {
            return _players[index];
        }

        public Connection GetOponnent(Connection plr)
        {
            return (GetPlayer(0) == plr ? GetPlayer(0) : GetPlayer(1));
        }
    }
}
