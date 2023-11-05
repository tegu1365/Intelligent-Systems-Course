using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HW0_62538
{
    internal class Program
    {
        public class JumpingFrogs
        {
            private string frogs;

            public string Frogs
            {
                get { return frogs; }
            }

            public JumpingFrogs(string fr)
            {
                frogs = fr;
            }

            public int EmptySlot()
            {
                return frogs.IndexOf("_");
            }

            public string ChangePositions(int index1, int index2)
            {
                char[] tempFrogs = frogs.ToCharArray();
                char temp = tempFrogs[index1];
                tempFrogs[index1] = tempFrogs[index2];
                tempFrogs[index2] = temp;
                return new string(tempFrogs);
            }

            public bool Equal(JumpingFrogs that)
            {
                return this.Frogs == that.Frogs;
            }

            public List<JumpingFrogs> GeneratePosibleMoves()
            {
                List<JumpingFrogs> nextJump = new List<JumpingFrogs>();
                int indexEmpty = this.EmptySlot();
                int size = frogs.Length;
                if (indexEmpty >= 2 && frogs[indexEmpty - 2] == '>')
                {
                    nextJump.Add(new JumpingFrogs(this.ChangePositions(indexEmpty - 2, indexEmpty)));
                }
                if (indexEmpty >= 1 && frogs[indexEmpty - 1] == '>')
                {
                    nextJump.Add(new JumpingFrogs(this.ChangePositions(indexEmpty - 1, indexEmpty)));
                }
                if (indexEmpty < size - 2 && frogs[indexEmpty + 2] == '<')
                {
                    nextJump.Add(new JumpingFrogs(this.ChangePositions(indexEmpty + 2, indexEmpty)));
                }
                if (indexEmpty < size - 1 && frogs[indexEmpty + 1] == '<')
                {
                    nextJump.Add(new JumpingFrogs(this.ChangePositions(indexEmpty + 1, indexEmpty)));
                }
                return nextJump;
            }

            public override string ToString()
            {
                return frogs;
            }
        }


        static List<JumpingFrogs> visited = new List<JumpingFrogs>();
        static bool AlreadyVisited(JumpingFrogs jumpingFrog)
        {
            foreach (var frogy in visited)
            {
                if (jumpingFrog.Equal(frogy))
                {
                    return true;
                }
            }
            return false;
        }

        static Stack<JumpingFrogs> stack = new Stack<JumpingFrogs>();
        static bool DFS(JumpingFrogs current, JumpingFrogs end)
        {
            if (current.Equal(end))
            {
                stack.Push(current);
                return true;
            }
            //posible optimisation: generate fewer posibilities (if already visited do not generate)
            List<JumpingFrogs> posibleMoves = current.GeneratePosibleMoves();
            foreach (var move in posibleMoves)
            {
                if (DFS(move, end) && !AlreadyVisited(move))
                {
                    stack.Push(move);
                    visited.Add(move);
                    return true;
                }
            }
            return false;
        }
        //exponential time 
        // 0.7s for N=15
        // 4.5s for N=17
        static void Main(string[] args)
        {
            //JumpingFrogs frogs=new JumpingFrogs(">>_<<");
            //Console.WriteLine(frogs.EmptySlot());
            //frogs.ChangePositions(0, 2);
            //Console.WriteLine(frogs);

            int N = Convert.ToInt32(Console.ReadLine());

            //Start stopwatch
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            JumpingFrogs start = new JumpingFrogs(new string('>', N) + "_" + new string('<', N));
            JumpingFrogs end = new JumpingFrogs(new string('<', N) + "_" + new string('>', N));
            //Console.WriteLine(start+"\n"+end);

            if (DFS(start, end))
            {
                //Stop Stopwatch
                //stopWatch.Stop();
                Console.WriteLine(start);
                while (stack.Count > 1)
                {
                    Console.WriteLine(stack.Pop().Frogs);
                }
                // Print time
                //TimeSpan ts = stopWatch.Elapsed;
                //string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds,
                //        ts.Milliseconds / 10);
                //Console.WriteLine("RunTime: " + elapsedTime);
            }
        }
    }
}