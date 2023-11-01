using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HW1_62538_TeodoraPetkova {

    public class Board
    {
        private int[,] board;
        private int nullTile;
        private int n;
        private string move;

        public Board(int[,] tiles, int N, int inpNullTile, string move="")
        {
            board = tiles;
            if (inpNullTile == -1)
            {
                nullTile = N;
            }
            else
            {
                nullTile = inpNullTile;
            }
            n = (int)Math.Sqrt(N + 1);
            this.move = move;
            // Console.WriteLine(goal);
        }
        public override string ToString()
        {
            string result = "";
            for (int i = 0;i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    result += board[i,j].ToString()+" ";
                }
                result += "\n";
            }
            return result;
        }
        public int TileAt(int row, int col)
        {
            return board[row, col];
        }
        public int TileAt(int index)
        {
            int row = index / n;
            int col = index % n;
            return board[row,col];
        }
        public int Size()
        {
            return n;
        }
        public int Manhattan()
        {
            int dist = 0;
            for(int i = 0;i<n;i++)
            {
                for(int j = 0; j < n; j++)
                {
                    int row = board[i, j] / n;
                    int col= board[i, j] % n;
                    dist += Math.Abs(row - i) + Math.Abs(col - j);
                }
            }
            return dist;
        }
        public bool IsGoal()
        {
            string goal="";
            int ind = 1;
            for (int i = 0; i < n*n + 1; i++)
            {
                if (i == nullTile)
                {
                    goal += "0 ";
                    ind = 0;
                }
                else
                {
                    goal += (i + ind).ToString() + " ";
                }
                if ((i + 1) % n == 0)
                {
                    goal += "\n";
                }
            }
            return this.ToString()==goal;
        }
        public bool Equals(Board y)
        {
            return y.ToString() == this.ToString();
        }
        public List<Board> Neighbors()
        {
            List<Board> list = new List<Board>();
            int row = nullTile / n;
            int col = nullTile % n;
            if(row+1 != n)
            {
                int[,] newBoard = board;
                int temp = board[row+1, col];
                board[row, col]=temp;
                board[row+1, col]=0;
                list.Add(new Board(newBoard, n * n, (n * (row+1) + col), "up"));
            }
            if (row - 1 != -1)
            {
                int[,] newBoard = board;
                int temp = board[row - 1, col];
                board[row, col] = temp;
                board[row - 1, col] = 0;
                list.Add(new Board(newBoard, n * n, (n * (row-1) + col), "down"));
            }
            if (col+1 != n)
            {
                int[,] newBoard = board;
                int temp = board[row, col+1];
                board[row, col] = temp;
                board[row, col+1] = 0;
                list.Add(new Board(newBoard, n * n, (n * row + (col+1)), "left"));
            }
            if (col - 1 != -1)
            {
                int[,] newBoard = board;
                int temp = board[row, col-1];
                board[row, col] = temp;
                board[row, col-1] = 0;
                list.Add(new Board(newBoard, n * n, (n * row + (col-1)), "right"));
            }
            return list;
        }
        public bool isSolvable()
        {
            int count = 0;
            int[] temp= board.Cast<int>().ToArray();
            for (int i = 0; i < n*n-1; i++)
            {
                for(int j = i+1;j < n*n; j++)
                {
                    if (temp[i] > temp[j] && temp[i] != 0 && temp[j] != 0)
                    {
                        count++;
                    }
                }
            }

            return ((count % 2 + 1) % 2)==1;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //reading input
            int N= Convert.ToInt32(Console.ReadLine());
            int inpNullTile=Convert.ToInt32(Console.ReadLine());
            //geting size NxN
            int size=(int)Math.Sqrt(N+1);
            //board
            int[,] inputBoard = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                var values = (Console.ReadLine().Split(' '));
                for (int j = 0; j < size; j++)
                {
                    inputBoard[i, j] = int.Parse(values[j]);
                }
            }

            //creating board
            Board board = new Board(inputBoard, 8, inpNullTile);
            //Console.WriteLine("_______________________________");
            Console.WriteLine(board);
            Console.WriteLine(board.isSolvable());
        }
    }

}