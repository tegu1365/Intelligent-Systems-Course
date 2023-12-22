using System.Diagnostics;

namespace HW2_62538
{
    class NQueens
    {
        char[,] board;
        List<ValueTuple<int, int>> queensPos;
        int n;
        public NQueens(int N)
        {
            board = new char[N, N];
            queensPos = new List<ValueTuple<int, int>>();
            n = N;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    board[i, j] = '_';
                }
            }
            Random rnd = new Random();
            for (int i = 0; i < n;)
            {
                int x = rnd.Next(0, N);
                board[i, x] = '*';
                queensPos.Add(new ValueTuple<int, int>(i, x));
                i++;
            }
        }

        public bool HasConflicts()
        {
            foreach (var pos in queensPos)
            {
                if (UnderAttack(pos))
                {
                    return true;
                }
            }
            return false;
        }

        #region Attack
        public bool AttackDiagonal(ValueTuple<int, int> pos)
        {
            foreach (var queen in queensPos)
            {
                if (Math.Abs(queen.Item1 - pos.Item1) == Math.Abs(queen.Item2 - pos.Item2)
                    && queen != pos)
                {
                    return true;
                }
            }
            return false;
        }
        public bool AttackRow(ValueTuple<int, int> pos)
        {
            foreach (var queen in queensPos)
            {
                if (queen.Item1 == pos.Item1 && queen != pos)
                {
                    return true;
                }
            }
            return false;
        }
        public bool AttackCol(ValueTuple<int, int> pos)
        {
            foreach (var queen in queensPos)
            {
                if (queen.Item2 == pos.Item2 && queen != pos)
                {
                    return true;
                }
            }
            return false;
        }
        public bool UnderAttack(ValueTuple<int, int> pos)
        {
            if (AttackDiagonal(pos))
            {
                return true;
            }
            if (AttackCol(pos))
            {
                return true;
            }
            if (AttackRow(pos))
            {
                return true;
            }
            return false;
        }
        #endregion

        public ValueTuple<int, int> PickRandomQueen()
        {
            //Random rnd= new Random();
            int index = 0; //rnd.Next(0, queensPos.Count());

            return queensPos[index];
        }

        public List<ValueTuple<int, int>> AvailablePositions(ValueTuple<int, int> pos)
        {
            List<ValueTuple<int, int>> availablePos = new List<(int, int)>();
            for (int i = 0; i < n; i++)
            {
                availablePos.Add((pos.Item1, i));
            }
            return availablePos;
        }

        public void MoveQueen(ValueTuple<int, int> startPos, ValueTuple<int, int> endPos)
        {
            if (board[startPos.Item1, startPos.Item2] == '*')
            {
                board[startPos.Item1, startPos.Item2] = '_';
                board[endPos.Item1, endPos.Item2] = '*';
                queensPos.Remove(startPos);
                queensPos.Add(endPos);
            }
        }
        public int NumberOfQueenConflicts(ValueTuple<int, int> pos)
        {
            if (queensPos.Contains(pos))
            {
                int count = 0;
                foreach (var queen in queensPos)
                {
                    if (queen.Item1 == pos.Item1 && queen != pos) count++;
                    if (queen.Item2 == pos.Item2 && queen != pos) count++;
                    if (Math.Abs(queen.Item1 - pos.Item1) == Math.Abs(queen.Item2 - pos.Item2)
                    && queen != pos) count++;
                }
                return count;
            }
            return -1;
        }
        public void PrintBoard()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(board[i, j]);
                }
                Console.WriteLine();
            }
        }
        public void PrintNums()
        {
            int count = 0;
            Console.Write("[");
            List<int> list = new List<int>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] == '*')
                    {
                        list.Add(j);
                    }
                }
            }
            string output = "";
            list.Reverse();
            list.ForEach(x => output += (x + ","));
            Console.Write(output.Remove(output.Length - 1));
            Console.Write("]");
        }
    }
    class Queens
    {
        int n;
        int[] queens;
        int[] queensRow;
        int[] queensD1;//main
        int[] queensD2;//secondery
        bool flag = false;
        Random rnd = new Random();


        public Queens(int n)
        {
            this.n = n;
            queens = new int[n];
            queensRow = new int[n];
            queensD1 = new int[n * 2 - 1];
            queensD2 = new int[n * 2 - 1];
        }

        public void InitQueens()
        {
            Random rnd = new Random();
            //queens = new int[]{ 2,3,2,1};
            for (int i = 0; i < n; i++)
            {
                queens[i] = rnd.Next(0, n);
                //queens[i]=getRowWithMinConflict(i);
                int temp = queens[i];
                queensRow[temp]++;
                queensD2[temp + i]++;
                queensD1[n - temp - 1 + i]++;
            }
        }

        public int NumberOfConflicts()
        {
            int result = 0;
            for (int i = 0; i < n * 2 - 1; i++)
            {
                int x = 0, y = 0, z = 0;
                if (i < n) x = queensRow[i];
                y = queensD2[i];
                z = queensD1[i];
                result += (x * (x - 1)) / 2;
                result += (y * (y - 1)) / 2;
                result += (z * (z - 1)) / 2;
            }
            return result;
        }

        internal int ColWithMaxConflict()
        {
            int maxConflicts = -1;
            List<int> col = new List<int> ();
            for (int i = 1; i < n; i++)
            {
                int temp = queens[i];
                int result = 0;
                int x = queensRow[temp];
                int y = queensD2[temp + i];
                int z = queensD1[n - temp - 1 + i];
                result += (x * (x - 1)) / 2;
                result += (y * (y - 1)) / 2;
                result += (z * (z - 1)) / 2;
                if (result >= maxConflicts)
                {
                    maxConflicts = result;
                    col.Add(i);
                }
            }
            if (maxConflicts == 0)
            {
                flag = true;
            }
            if (col != null)
            {
                if (col.Count <= 1)
                {
                    return col[0];
                }
                int index = rnd.Next(0, col.Count);
                return col[index];
            }
            return -1;
        }

        internal int getRowWithMinConflict(int col)
        {
            int minConflicts = int.MaxValue;
            List<int> row=new List<int> ();
            for (int i = 0; i < n; i++)
            {
                int result = 0;
                int x = queensRow[i];
                int y = queensD2[i + col];
                int z = queensD1[n - i - 1 + col];
                result += (x * (x - 1)) / 2;
                result += (y * (y - 1)) / 2;
                result += (z * (z - 1)) / 2;
                if (result <= minConflicts)
                {
                    minConflicts = result;
                    row.Add(i);
                }
            }
            if (row.Count == 1)
            {
                return row[0];
            }
            int index = rnd.Next(0, row.Count);
            return row[index];
        }

        internal void Move(int col, int newRow)
        {
            int oldRow = queens[col];
            queensRow[oldRow]--;
            queensD2[oldRow + col]--;
            queensD1[n - oldRow - 1 + col]--;

            queens[col] = newRow;
            queensRow[newRow]++;
            queensD2[newRow + col]++;
            queensD1[n - newRow - 1 + col]++;
        }

        internal void PrintBoard()
        {
            char[,] board=new char[n,n];
            for(int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    board[i, j] = '_';
                }
            }
            for(int i = 0; i < n; i++)
            {
                int j = queens[i];
                board[i, j] = '*';
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(board[i,j]);
                }
                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            string str = "[";
            for(int i = 0; i < n; i++)
            {
                if (i != n - 1)
                {
                    str += $"{queens[i]},";
                }
                else
                {
                    str += $"{queens[i]}]";
                }
            }
            return str;
        }
        public bool Flag
        {
            get { return flag; }
        }
    }

    internal class Program
    {
        static int k = (int) Math.Pow(10, 6);//1 000 000
        //NQueen too random can have infinit loop
        static void FirstTypeSolving(int N)
        {
            NQueens queens = new NQueens(N);
            int i = 0;
            while (queens.HasConflicts())
            {
                int minAttacks = N + 1;
                ValueTuple<int, int> randomQueen = queens.PickRandomQueen();
                var positions = queens.AvailablePositions(randomQueen);
                ValueTuple<int, int> minConflictPosition = (-1, -1);
                foreach (var pos in positions)
                {
                    queens.MoveQueen(randomQueen, pos);
                    int newNumOfConflicts = queens.NumberOfQueenConflicts(pos);
                    if (newNumOfConflicts < minAttacks)
                    {
                        minConflictPosition = pos;
                        minAttacks = newNumOfConflicts;
                    }
                    queens.MoveQueen(pos, randomQueen);

                }
                queens.MoveQueen(randomQueen, minConflictPosition);
                //i++;
                //Console.WriteLine($"Itaration {i}");
                //queens.PrintBoard();
            }

            // queens.PrintBoard();
            queens.PrintNums();
        }
        static Queens queens;
        //1D array type of solving
        static void OneDArraySolving(int N)
        {
            queens = new Queens(N);
            queens.InitQueens();
            int iter = 0;
            while (iter++ <= N*2)
            {
                int col = queens.ColWithMaxConflict();
                int row = queens.getRowWithMinConflict(col);
                //Console.WriteLine(iter+" | "+col+": "+row);
                queens.Move(col,row);
                if (queens.Flag)
                {
                    break;
                }
            }

            if(queens.NumberOfConflicts() > 0)
            {
                OneDArraySolving(N);
            }
            else
            {
                return;
               
            }
        }

        //static void Main(String[] args)
        //{
        //    int N = Convert.ToInt32(Console.ReadLine());
        //    if (N == 1)
        //    {
        //        //Console.WriteLine("*");
        //        //Console.WriteLine("[0]");
        //    }
        //    if (N==0||N==2||N==3)
        //    {
        //        Console.WriteLine(-1);
        //    }
        //    else
        //    {

        //        //Start stopwatch
        //        Stopwatch stopWatch = new Stopwatch();
        //        stopWatch.Start();

        //        //may be backtrack somewhere...to get faster
        //        //for the moment is still too inconsistent
        //        OneDArraySolving(N);
        //        //Stop Stopwatch
        //        stopWatch.Stop();


        //        queens.PrintBoard();
        //        Console.WriteLine(queens);

        //        //Print time
        //        TimeSpan ts = stopWatch.Elapsed;
        //        string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds,
        //                ts.Milliseconds / 10);
        //        Console.WriteLine("RunTime: " + elapsedTime);
        //    }
        //}
    }
}