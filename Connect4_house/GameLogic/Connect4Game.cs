using Connect4_house.GameLogic.Structures;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4_house.GameLogic
{
    internal class Connect4Game
    {
        private const char EMPTYSPACE = '*';
        private const char ENEMY = 'R';
        private const char TEAM = 'O';
        private const int ROW_MAX = 6;
        private const int COL_MAX = 7;

        private int[] _slotIndices;
        private char[,] _board;

        public Connect4Game()
        {
            _board = new char[ROW_MAX, COL_MAX];
            ResetBoard();
        }

        private bool CheckSequence(int row, int col, int dRow, int dCol, char p)
        {
            int count = 0;

            while (row < ROW_MAX && col < COL_MAX && count < 4)
            {
                if (_board[row, col] == p)
                    count++;
                else
                    break;

                row += dRow;
                col += dCol;
            }

            return count == 4;
        }

        public bool IsDraw()
            => _slotIndices.All(x => x == ROW_MAX);

        public bool FindConsecutive4(PlayerType pType)
        {
            char c = pType == PlayerType.RED ? ENEMY : TEAM;

            for (int rowi = 0; rowi < _board.GetLength(0); rowi++)
            {
                for (int colj = 0; colj < _board.GetLength(1); colj++)
                {
                    //rows
                    if (CheckSequence(rowi, colj, 0, 1, c))
                    {
                        return true;
                    }
                    //columns
                    if (CheckSequence(rowi, colj, 1, 0, c))
                    {
                        return true;
                    }
                    //diagonal
                    if (CheckSequence(rowi, colj, 1, 1, c))
                    {
                        return true;
                    }
                    //reverse diagonal
                    if (CheckSequence(rowi, colj, -1, 1, c))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ResetBoard()
        {
            _slotIndices = new int[7];
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for(int y = 0; y < _board.GetLength(1); y++)
                {
                    _board[i, y] = EMPTYSPACE;
                }
            }
        }

        public bool CanDropAtSlot(int slot)
            => _slotIndices[slot - 1] < ROW_MAX;

        public void DropCoin(int slot, PlayerType pType)
        {
            int row = (ROW_MAX - 1) - _slotIndices[slot - 1]++;
            int col = slot - 1;
            char c = pType == PlayerType.RED ? ENEMY : TEAM;
            _board[row, col] = c;
        }

        public void PrintBoard()
        {
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int y = 0; y < _board.GetLength(1); y++)
                {
                    Console.Write(_board[i, y]);
                }
                Console.WriteLine();
            }
        }

        private string GetDiscordEmojiFromCharacter(char c)
        {
            if (c == ENEMY)
                return "🔴";
            else if (c == TEAM)
                return "🟡";

            return "⚫";
        }

        public StringBuilder GetDiscordBoard()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("1️⃣2️⃣3️⃣4️⃣5️⃣6️⃣7️⃣");
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int y = 0; y < _board.GetLength(1); y++)
                {
                    sb.Append(GetDiscordEmojiFromCharacter(_board[i, y]));
                }
                sb.AppendLine();
            }
            return sb;
        }
    }
}
