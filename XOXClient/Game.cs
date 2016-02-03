using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOXClient
{
    class Game
    {
        private static byte[] _fields = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static byte DoMovement(int index)
        {
            Console.WriteLine("Enter a field id (1-9):");
            int field = 1;
            bool invalidInput = true;
            do
            {
                field = Convert.ToInt32(Console.ReadLine());
                invalidInput = !(field < 10 && field > 0 && _fields[field - 1] == 0);
                Console.WriteLine("Wrong input, try again (1-9):");
            } while (invalidInput);

            return Convert.ToByte(field);
        }

        public static string DrawTable()
        {
            string str = String.Empty;
            for (int i = 0; i < 9; ++i)
            {
                str += (_fields[i] == 1 ? "X" : (_fields[i] == 2 ? "O" : Convert.ToString(i+1)));
                if (i != 2 && i != 5 && i != 8)
                    str += "|";
                else str += "\n" + (i != 8 ? "------\n" : String.Empty);
            }
            return str;
        }

        public static int FindWinner()
        {
            for (int i = 1; i < 3; ++i)
            {
                if ((_fields[0] == i && _fields[1] == i && _fields[2] == i) ||
                    (_fields[3] == i && _fields[4] == i && _fields[5] == i) ||
                    (_fields[6] == i && _fields[7] == i && _fields[8] == i) ||
                    (_fields[0] == i && _fields[4] == i && _fields[8] == i) ||
                    (_fields[2] == i && _fields[4] == i && _fields[6] == i) ||
                    (_fields[0] == i && _fields[3] == i && _fields[6] == i) ||
                    (_fields[1] == i && _fields[4] == i && _fields[7] == i) ||
                    (_fields[2] == i && _fields[5] == i && _fields[8] == i))
                {
                    return i;
                }
            }
            return 0;
        }

        public static bool GameInProgress()
        {
            for (int i = 0; i < 3; ++i)
                if (_fields[i] != 0)
                    return true;

            return false;
        }

        public static int GetMyIndex(int opponentIndex)
        {
            return (opponentIndex == 1 ? 2 : 1);
        }

        public static void Reset()
        {
            for (int i = 0; i < 3; ++i)
                _fields[i] = 0;
        }

        public static void UpdateFields(byte field, int opponentIndex)
        {
            _fields[field - 1] = Convert.ToByte(opponentIndex);
        }
    }
}
