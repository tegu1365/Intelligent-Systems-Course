using System;
namespace HW4_62538
{
    public class Program
    {
        static char[,] board = new char[3, 3] { { ' ', ' ', ' ' }, { ' ', ' ', ' ' }, { ' ', ' ', ' ' } };
        static char currentPlayer = 'X';
        static char firstPlayer = 'P';
        static char computerChar = 'O';
        static char playerChar = 'X';

        static void Main()
        {
            Console.Write("Who would be the first the computer (C) or the player (P): ");
            string[] input = Console.ReadLine().Split();
            if (input[0] == "C")
            {
                firstPlayer = 'C';
            }
            else if (input[0] != "P")
            {
                Console.WriteLine("Invalid input!");
                Main();
            }

            while (true)
            {
                PrintBoard();
                if (firstPlayer == 'P')
                {
                    playerChar = 'X';
                    computerChar = 'O';
                    if (currentPlayer == 'X')
                    {
                        PlayerMove();
                    }
                    else
                    {
                        ComputerMove();
                    }
                }
                else
                {
                    playerChar = 'O';
                    computerChar = 'X';
                    if (currentPlayer == 'X')
                    {
                        ComputerMove();
                    }
                    else
                    {
                        PlayerMove();
                    }
                }

                char result = CheckGameResult(board);
                if (result != ' ')
                {
                    PrintBoard();
                    if (result == 'D')
                    {
                        Console.WriteLine("It's a draw!");
                    }
                    else
                    {
                        Console.WriteLine($"{result} wins!");
                    }
                    break;
                }

                SwitchPlayer();
            }
        }

        static void PrintBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write($"{board[i, j]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void PlayerMove()
        {
            int row, col;
            do
            {
                Console.Write("Enter your move (row and column (starts from 1), separated by space): ");
                string[] input = Console.ReadLine().Split();
                row = int.Parse(input[0]) - 1;
                col = int.Parse(input[1]) - 1;
            } while (board[row, col] != ' ');
            board[row, col] = playerChar;
            
        }

        static void ComputerMove()
        {
            (int, int) bestMove = FindBestMove();
            board[bestMove.Item1, bestMove.Item2] = computerChar;
            Console.WriteLine($"Computer plays ({bestMove.Item1 + 1}, {bestMove.Item2 + 1})");
        }

        static (int, int) FindBestMove()
        {
            int bestVal = int.MinValue;
            (int, int) bestMove = (-1, -1);

            (int winRow, int winCol) = FindImmediateWin();
            if (winRow != -1 && winCol != -1)
            {
                return (winRow, winCol);
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        board[i, j] = computerChar;
                        int moveVal = Minimax(board, 0, false, int.MinValue, int.MaxValue);
                        board[i, j] = ' ';

                        if (moveVal > bestVal)
                        {
                            bestMove = (i, j);
                            bestVal = moveVal;
                        }
                    }
                }
            }

            return bestMove;
        }

        static (int, int) FindImmediateWin()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        board[i, j] = computerChar;
                        if (CheckGameResult(board) == computerChar)
                        {
                            board[i, j] = ' ';
                            return (i, j); 
                        }
                        board[i, j] = ' ';
                    }
                }
            }
            return (-1, -1);
        }

        static int Minimax(char[,] board, int depth, bool isMaximizing, int alpha, int beta)
        {
            char result = CheckGameResult(board);
            if (result != ' ')
            {
                if (result == playerChar) return -1;
                else if (result == computerChar) return 1;
                else return 0;
            }

            //if (depth >= 4)//tried it makes wrong desisions
            //{
            //    return 0;
            //}

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board[i, j] == ' ')
                        {
                            board[i, j] = computerChar;
                            int eval = Minimax(board, depth + 1, false, alpha, beta);
                            board[i, j] = ' ';
                            //eval-=depth;//tried this didn't work made the algo worse
                            maxEval = Math.Max(maxEval, eval);
                            alpha = Math.Max(alpha, eval);
                            if (beta <= alpha) break;
                        }
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board[i, j] == ' ')
                        {
                            board[i, j] = playerChar;
                            int eval = Minimax(board, depth + 1, true, alpha, beta);
                            board[i, j] = ' ';
                            //eval+= depth;
                            minEval = Math.Min(minEval, eval);
                            beta = Math.Min(beta, eval);
                            if (beta <= alpha) break;
                        }
                    }
                }
                return minEval;
            }
        }

        static char CheckGameResult(char[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 0] != ' ')
                {
                    return board[i, 0];
                }

                if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && board[0, i] != ' ')
                {
                    return board[0, i];
                }
            }

            if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[0, 0] != ' ')
            {
                return board[0, 0];
            }

            if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[0, 2] != ' ')
            {
                return board[0, 2];
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        return ' ';
                    }
                }
            }

            return 'D';
        }

        static void SwitchPlayer()
        {
            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
        }
    }

}