using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HW0_62538
{
    public class JumpingFrogs
    {
        private string frogs;
       
        public string Frogs
        {
            get { return frogs; }
        }

        public JumpingFrogs(string fr) {
            frogs = fr;
        }

        public int EmptySlot()
        {
            return frogs.IndexOf("_");
        }

        public string ChangePositions(int index1,int index2)
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
            List<JumpingFrogs> nextJump=new List<JumpingFrogs>();
            int indexEmpty=this.EmptySlot();
            int size = frogs.Length;
            if(indexEmpty >= 2 && frogs[indexEmpty - 2] == '>')
            {
                nextJump.Add(new JumpingFrogs(this.ChangePositions(indexEmpty - 2, indexEmpty)));
            }
            if (indexEmpty >= 1 && frogs[indexEmpty - 1] == '>')
            {
                nextJump.Add(new JumpingFrogs(this.ChangePositions(indexEmpty - 1, indexEmpty)));
            }
            if (indexEmpty < size-2 && frogs[indexEmpty + 2] == '<')
            {
                nextJump.Add(new JumpingFrogs(this.ChangePositions(indexEmpty + 2, indexEmpty)));
            }
            if (indexEmpty <size- 1 && frogs[indexEmpty + 1] == '<')
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
    public class Node
    {
        JumpingFrogs frogs;
        Node prev;
        public Node(JumpingFrogs frogs, Node prev)
        {
            this.frogs = frogs;
            this.prev = prev;
        }
        public JumpingFrogs Frogs{
            get{return frogs;}
        }
        public Node Prev
        {
            get { return prev;}
        }
    }
    internal class Program
    {
        static List<JumpingFrogs> visited=new List<JumpingFrogs>();
        static bool AlreadyVisited(JumpingFrogs jumpingFrog)
        {
            foreach(var frogy in visited)
            {
                if (jumpingFrog.Equal(frogy))
                {
                    return true;
                }
            }
            return false;
        }
      
        static Stack<Node> stack = new Stack<Node>();
        static Node DFS(Node current, JumpingFrogs end)
        {
            if (current.Frogs.Equal(end))
            {
                stack.Push(current);
                return current;
            }
            List<JumpingFrogs> posibleMoves=current.Frogs.GeneratePosibleMoves();
            foreach(var move in posibleMoves)
            {
                if(DFS(new Node(move,current), end)!=null&&!AlreadyVisited(move))
                {
                    stack.Push(new Node(move, current));
                    visited.Add(move);
                    return new Node(move, current);
                }
            }
            return null;
        } 
        static void Main(string[] args)
        {
            //JumpingFrogs frogs=new JumpingFrogs(">>_<<");
            //Console.WriteLine(frogs.EmptySlot());
            //frogs.ChangePositions(0, 2);
            //Console.WriteLine(frogs);

            int N=Convert.ToInt32(Console.ReadLine());
            JumpingFrogs start= new JumpingFrogs(new string('>',N)+"_"+new string('<',N));
            JumpingFrogs end= new JumpingFrogs(new string('<', N) + "_" + new string('>', N));
            //Console.WriteLine(start+"\n"+end);
            
            Node ans=DFS(new Node(start,null), end);
            Console.WriteLine(start);
            while (stack.Count > 1)
            {
                Console.WriteLine(stack.Pop().Frogs);
            }
        }
    }
}