using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace HW1_62538_TeodoraPetkova
{


    public class Board
    {
        private int[,] board;
        private int nullTile;
        private int n;
        private string move;

        public string Move
        {
            get { return move; }
        }

        public Board(int[,] tiles, int N, int inpNullTile, string move = "")
        {

            if (inpNullTile == -1)
            {
                nullTile = N;
            }
            else
            {
                nullTile = inpNullTile;
            }
            n = (int)Math.Sqrt(N + 1);
            board = new int[n, n];
            board = getTilesCopy(tiles);
            this.move = move;
            //Console.WriteLine(this);
        }
        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    result += board[i, j].ToString() + " ";
                }
                result += "\n";
            }
            return result;
        }
        public int TileAt(int row, int col)
        {
            return board[row, col];
        }
        public int Size()
        {
            return n;
        }
        public int Manhattan()
        {
            int dist = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] != 0)
                    {
                        int row = (board[i, j] - 1) / n;
                        int col = (board[i, j] - 1) % n;
                        dist += Math.Abs(row - i) + Math.Abs(col - j);
                    }
                }
            }
            return dist;
        }
        public bool IsGoal()
        {
            string goal = "";
            int ind = 1;
            for (int i = 0; i < n * n; i++)
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
            // Console.WriteLine("GOAL:\n" + goal);
            return this.ToString() == goal;
        }
        public bool Equals(Board y)
        {
            return y.ToString() == this.ToString();
        }

        int[,] getTilesCopy(int[,] a)
        {
            int[,] b = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    b[i, j] = a[i, j];
                }
            }
            return b;
        }
        public List<Board> Neighbors()
        {
            List<Board> list = new List<Board>();
            int currentNull = 0;
            bool flag = false;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] == 0)
                    {
                        flag = true;
                        break;
                    }
                    currentNull++;
                }
                if (flag)
                {
                    break;
                }
            }
            int row = currentNull / n;
            int col = currentNull % n;
            //Console.WriteLine($"Current Null: {currentNull} on [{row},{col}]");
            
            //the movements are like in the examples opposit of the 0 element move
            if (row + 1 != n)
            {
                int[,] newBoard = new int[n, n];
                newBoard = getTilesCopy(board);

                int temp = board[row + 1, col];
                newBoard[row, col] = temp;
                newBoard[row + 1, col] = 0;
                Board tempBoard = new Board(newBoard, n * n, nullTile, "up");
                list.Add(tempBoard);
                Console.WriteLine("UP:\n" + tempBoard);
            }
            if (row - 1 != -1)
            {
                int[,] newBoard = new int[n, n];
                newBoard = getTilesCopy(board);
                int temp = board[row - 1, col];
                newBoard[row, col] = temp;
                newBoard[row - 1, col] = 0;
                Board tempBoard = new Board(newBoard, n * n, nullTile, "down");
                list.Add(tempBoard);
                Console.WriteLine("DOWN:\n" + tempBoard);
            }
            if (col + 1 != n)
            {
                int[,] newBoard = new int[n, n];
                newBoard = getTilesCopy(board);
                int temp = board[row, col + 1];
                newBoard[row, col] = temp;
                newBoard[row, col + 1] = 0;
                Board tempBoard = new Board(newBoard, n * n, nullTile, "left");
                list.Add(tempBoard);
                Console.WriteLine("LEFT: \n" + tempBoard);
            }
            if (col - 1 != -1)
            {
                int[,] newBoard = new int[n, n];
                newBoard = getTilesCopy(board);
                int temp = board[row, col - 1];
                newBoard[row, col] = temp;
                newBoard[row, col - 1] = 0;
                Board tempBoard = new Board(newBoard, n * n, nullTile, "right");
                list.Add(tempBoard);
                Console.WriteLine("RIGHT:\n" + tempBoard);
            }
            return list;
        }
        public bool isSolvable()
        {
            int count = 0;
            int[] temp = board.Cast<int>().ToArray();
            for (int i = 0; i < n * n - 1; i++)
            {
                for (int j = i + 1; j < n * n; j++)
                {
                    if (temp[i] > temp[j] && temp[i] != 0 && temp[j] != 0)
                    {
                        count++;
                    }
                }
            }

            return ((count % 2 + 1) % 2) == 1;
        }
    }

    public class Node
    {
        private Board board;
        private int dist, moves;
        private Node prev;

        public Node(Board board, int moves, Node prev)
        {
            this.board = board;
            this.dist = board.Manhattan();
            this.moves = moves;
            this.prev = prev;
        }

        public int Dist
        {
            get { return dist; }
        }
        public int Moves
        {
            get { return moves; }
        }
        public Board Board { get { return board; } }
        public Node Prev { get { return prev; } }
    }

    public class NodeQueueCompearer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            return (x.Moves + x.Dist) - (y.Moves + y.Dist);
        }
    }

    public class SolverAStar
    {
        private System.Collections.Generic.PriorityQueue<Node, Node> priorityQueue = new System.Collections.Generic.PriorityQueue<Node, Node>(new NodeQueueCompearer());
        private int minMoves = -1;
        private Node bestestOne;

        public SolverAStar(Board startBoard)
        {
            if (startBoard == null)
            {
                Console.WriteLine(-1);
                return;
            }
            Node newNode = new Node(startBoard, 0, null);
            priorityQueue.Enqueue(newNode, newNode);

            while (priorityQueue.Count > 0)
            {
                Node current = priorityQueue.Dequeue();
                //Console.WriteLine(current.Board + "\n_______________________________________");
                if (current.Board.IsGoal())
                {
                    if (minMoves == -1 || current.Moves < minMoves)
                    {
                        minMoves = current.Moves;
                        bestestOne = current;
                    }
                }

                if (minMoves == -1 || current.Moves + current.Moves < minMoves)
                {
                    List<Board> versions = current.Board.Neighbors();
                    foreach (Board b in versions)
                    {
                        if (current.Prev == null || !b.Equals(current.Prev.Board))
                        {
                            newNode = new Node(b, current.Moves + 1, current);
                            priorityQueue.Enqueue(newNode, newNode);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public int Moves
        {
            get { return minMoves; }
        }
        public void Path()
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(bestestOne);
            Node current = bestestOne;
            while (current.Prev != null)
            {
                current = current.Prev;
                stack.Push(current);
            }
            while (stack.Count > 0)
            {
                Node output = stack.Pop();
                if (output.Board.Move != "")
                {
                    Console.WriteLine(output.Board.Move);
                }
            }
        }
    }

    public class SolverIDA
    {
        private int minMoves = -1;
        private Node bestestOne;

        public SolverIDA(Board startBoard)
        {
            minMoves = IDAStar(new Node(startBoard, 0, null));
        }
        public int Moves
        {
            get { return minMoves; }
        }
        public int IDAStar(Node current)
        {
            if (current == null) return -1;
            int threshold = current.Dist;

            while (true)
            {
                int dist = IDA(current, threshold);
                if (dist == int.MaxValue)
                {
                    return -1;
                }
                else
                {
                    if(dist<0){
                        //minMoves = current.Moves;
                        return -dist;
                    }
                    else
                    {
                        threshold = dist;
                    }
                }
            }
        }
        public int IDA(Node current, int threshold)
        {
            if (current.Board.IsGoal())
            {
                bestestOne = current;
                return -current.Moves;
            }

            int estimate = current.Moves + current.Dist;

            if (estimate > threshold)
            {
                return estimate;
            }

            int min = int.MaxValue;
            List<Board> versions = current.Board.Neighbors();

            foreach (Board b in versions)
            {
                int t = IDA(new Node(b, current.Moves + 1, current), threshold);
                if (t < 0)
                {
                    return t;
                }
                else if (t < min)
                {
                    min = t;
                }
            }
            return min;
        }
        public void Path()
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(bestestOne);
            Node current = bestestOne;
            while (current.Prev != null)
            {
                current = current.Prev;
                stack.Push(current);
            }
            while (stack.Count > 0)
            {
                Node output = stack.Pop();
                if (output.Board.Move != "")
                {
                    Console.WriteLine(output.Board.Move);
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {

            //reading input
            int N = Convert.ToInt32(Console.ReadLine());
            int inpNullTile = Convert.ToInt32(Console.ReadLine());
            //geting size NxN
            int size = (int)Math.Sqrt(N + 1);
            //board
            int[,] inputBoard = new int[size, size];
            //Input needs to be like in the exmples 
            // N
            // nullTile in the end
            // N numbers for N rows
            for (int i = 0; i < size; i++)
            {
                var values = (Console.ReadLine().Split(' '));
                for (int j = 0; j < size; j++)
                {
                    inputBoard[i, j] = int.Parse(values[j]);
                }
            }
            //Start stopwatch
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //creating board
            Board board = new Board(inputBoard, N, inpNullTile);
            //Console.WriteLine("_______________________________");
            if (board.isSolvable())
            {

                //board.Neighbors();

                SolverIDA solver = new SolverIDA(board);
                //Stop Stopwatch
                stopWatch.Stop();

                Console.WriteLine(solver.Moves);
                solver.Path();
                //Print time
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds,
                        ts.Milliseconds / 10);
                Console.WriteLine("RunTime: " + elapsedTime);
            }
            else
            {
                Console.WriteLine(-1);
            }
        }
    }

}