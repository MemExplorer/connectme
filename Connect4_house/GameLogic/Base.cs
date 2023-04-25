using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4_house.GameLogic
{
    internal class Base
    {
        private char[,] dots;

        private int[][] directions;

        private const int ROW_MAX = 5;
        private const int COL_MAX = 7;

        public Base() {
            dots = new char[ROW_MAX,COL_MAX];

            directions =  new int[][]
            {
               new int[] {1, 0},   //right
               new int[] {0, 1},    // up
               new int[] {-1, 0},   //left
               new int[]{0, -1},   //down
               new int[]{1, -1},   //right- down (diagonal)
               new int[]{-1, 1},   // left - up (diagonal)
               new int[]{-1, -1},  // left - down (diagonal)
                new int[]{1, 1}, // right - up (diagonal)
            };
        }


        public void Insert(char color, int positionRow, int positionCol)
        {
            dots[positionRow, positionCol] = color;

        }
        

        //will start at the last inserted position and grow in each possible direction
        public bool VerifyConnect4(int positionRow, int positionCol, char mustValue)
        {

            foreach(int[] direction  in directions)
            {
                int posRow = direction[0] + positionRow;
                int posCol = direction[1] +positionCol;

                if (Traverse4(posRow, posCol, 1, direction, mustValue))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Traverse4(int posRow, int posCol, int count, int[] direction, char mustValue)
        {
           if(!InBounds(posRow, posCol))
            {
                return false;
            }
           else if (dots[posRow, posCol] != mustValue)
            {
                return false;
            }
           else if (count == 4)
            {
                return true;
            }
            

           return Traverse4(posRow + direction[0], posCol + direction[1], count + 1, direction, mustValue);
        }

        private bool InBounds(int posRow, int posCol)
        {

            return posRow >= 0 && posRow < ROW_MAX && posCol >= 0 && posCol < COL_MAX;
        }




    }
}
