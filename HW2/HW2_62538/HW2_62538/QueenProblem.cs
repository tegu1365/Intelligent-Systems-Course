using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW2_62538
{
    internal class QueenProblem
    {
        static void Main()
        {
            int n = 100;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int[] solution = SolveNQueens(n);

            stopwatch.Stop();

            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");

            if (n <= 100)
            {
                PrintSolution(solution);
            }
        }

        static int[] SolveNQueens(int n)
        {
            int[] solution = new int[n];
            SolveNQueensUtil(solution, 0, n);
            return solution;
        }

        static bool SolveNQueensUtil(int[] solution, int row, int n)
        {
            if (row == n)
            {
                return true;
            }

            for (int i = 0; i < n; i++)
            {
                if (IsSafe(solution, row, i))
                {
                    solution[row] = i;

                    if (SolveNQueensUtil(solution, row + 1, n))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsSafe(int[] solution, int row, int col)
        {
            for (int i = 0; i < row; i++)
            {
                if (solution[i] == col || Math.Abs(solution[i] - col) == Math.Abs(i - row))
                {
                    return false;
                }
            }

            return true;
        }

        static void PrintSolution(int[] solution)
        {
            for (int i = 0; i < solution.Length; i++)
            {
                for (int j = 0; j < solution.Length; j++)
                {
                    Console.Write(solution[i] == j ? "* " : "_ ");
                }
                Console.WriteLine();
            }
        }
    }

}