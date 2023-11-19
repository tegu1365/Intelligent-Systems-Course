using System.Drawing;
using System.Security.AccessControl;

namespace HW3_62538
{
    internal class Program
    {
        static List<(int Index, int Weight, int Value)> items = new List<(int Index, int Weight, int Value)>();
        static int maxBackpack, numberOfItems;
        const double MUTATION_RATE = 0.45;
        const int MAX_GENERATIONS_WITHOUT_NEW_BEST = 500;
        const int MAX_GENERATIONS_REPEATS = 400;
        static int sizeOfPopulation = 150;
        //static List<(List<Backpack> Population, int max)> generations = new List<(List<Backpack> Population, int max)>();
        static Random rnd = new Random();
        static List<int> gens=new List<int>();

        public class Backpack
        {

            private int[] bits;

            public int[] Bits
            {
                get
                {
                    int[] temp = new int[bits.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = bits[i];
                    }
                    return temp;
                }
                set
                {
                    if (value != null)
                    {
                        bits = new int[value.Length];
                        for (int i = 0; i < bits.Length; i++)
                        {
                            bits[i] = value[i];
                        }
                    }
                }
            }

            public Backpack()
            {
                Bits = Enumerable.Repeat(0, numberOfItems).ToArray();
            }
            public Backpack(int[] b)
            {
                Bits = b;
            }
            public Backpack(Backpack backpack)
            {
                Bits = backpack.Bits;
            }

            public int Fitness()
            {
                int total_mass = items.Where(x => bits[x.Index] == 1).Sum(x => x.Weight);
                if (total_mass > maxBackpack)
                {
                    return 0;
                }
                int total_value = items.Where(x => bits[x.Index] == 1).Sum(x => x.Value);
                return total_value;
            }
            public int Mass()
            {
                int total_mass = items.Where(x => bits[x.Index] == 1).Sum(x => x.Weight);
                return total_mass;
            }
            public override string ToString()
            {
                string str = "";
                for (int i = 0; i < bits.Length; i++)
                {
                    str += $"{bits[i]} ";
                }
                str += $"{this.Fitness()}";
                return str;
            }
        }

        static int[] GenerateBits()
        {
            int[] bits = new int[numberOfItems];
            bits = Enumerable.Repeat(0, bits.Length).Select(i => rnd.Next(0, 2)).ToArray();
            return bits;
        }

        static List<Backpack> Init(int size)
        {
            //Console.WriteLine("Population 0:");
            List<Backpack> population = new List<Backpack>();
            for (int i = 0; i < size; i++)
            {
                Backpack backpack = new Backpack(GenerateBits());
                population.Add(backpack);
                //Console.WriteLine(backpack);
            }
            return population;
        }

        static int MaxInPopulation(List<Backpack> population)
        {
            var output = population.MaxBy(x => x.Fitness());
            //Console.WriteLine("Max Backpack for the current population:");
            //Console.WriteLine(output);
            return output.Fitness();
        }

        static void PrintPopulation(List<Backpack> population)
        {
            for (int i = 0; i < population.Count; i++)
            {
                Console.WriteLine(population[i]);
            }
        }

        static List<Backpack> Selection(List<Backpack> population)
        {
            //var rndPopulation = population.OrderBy(x => rnd.Next()).ToList();
            int[] index = Enumerable.Repeat(0, 4).Select(x => rnd.Next(sizeOfPopulation)).ToArray();
            List<Backpack> parents = new List<Backpack>();
            //Console.WriteLine("RND population:");
            //PrintPopulation(rndPopulation);
            if (population[index[0]].Fitness() > population[index[1]].Fitness())
            {
                //if (population[index[0]].Fitness() != 0)
                    parents.Add(population[index[0]]);
            }
            else
            {
                //if (population[index[1]].Fitness() != 0)
                    parents.Add(population[index[1]]);
            }
            if (population[index[2]].Fitness() > population[index[3]].Fitness())
            {
                //if (population[index[2]].Fitness() != 0)
                    parents.Add(population[index[2]]);
            }
            else
            {
                //if (population[index[3]].Fitness() != 0)
                    parents.Add(population[index[3]]);
            }
            //if (parents.Count < 2)
            //{
            //    return Selection(population);
            //}
            return parents;
        }

        static int[] CreateChild(List<Backpack> parents, int k)
        {
            int[] bits = new int[numberOfItems];
            int N = numberOfItems / 2;
            int a, b;
            k = k % 2;
            if (k == 0)
            {
                a = 0;
                b = 1;
            }
            else
            {
                a = 1;
                b = 0;
            }
            int[] parentA = parents[a].Bits;
            int[] parentB = parents[b].Bits;
            for (int i = 0; i < N; i++)
            {
                bits[i] = parentA[i];
            }
            for (int i = N + 1; i < numberOfItems; i++)
            {
                bits[i] = parentB[i];
            }
            return bits;
        }

        static List<Backpack> Crossover(List<Backpack> parents)
        {
            List<Backpack> children = new List<Backpack>();
            children.Add(new Backpack(CreateChild(parents, 0)));
            children.Add(new Backpack(CreateChild(parents, 1)));
            return children;
        }

        static void Mutation(List<Backpack> children)
        {
            foreach (var child in children)
            {
                int[] childBits = child.Bits;
                for (int i = 0; i < child.Bits.Length; i++)
                {
                    if (rnd.NextDouble() < MUTATION_RATE)
                    {
                        childBits[i] = childBits[i] == 0 ? 1 : 0;
                    }
                }
                child.Bits = childBits;
            }
        }

        //Maybe more children per Gen need more new variations
        static List<Backpack> NextGeneration(List<Backpack> population)
        {
            List<Backpack> nextGen = new List<Backpack>();

            List<Backpack> parents = Selection(population);
            List<Backpack> children = Crossover(parents);
            if (rnd.NextDouble() < MUTATION_RATE)
            {
                Mutation(children);
            }

            nextGen.AddRange(children);
            population = population.OrderBy(x => rnd.Next()).ToList();
            for (int i = 2; i < sizeOfPopulation; i++)
            {
                nextGen.Add(population[i]);
            }
            return nextGen;
        }
        
        static int Solve()
        {
            List<Backpack> population = new List<Backpack>();
            population = Init(sizeOfPopulation);

            int currentMax = MaxInPopulation(population);
            //generations.Add((population, currentMax));
            
            int prevNum = MaxInPopulation(population);
            int numberOfGensWithoutNewBest = 0;
            int repeatingNumber = 0;
            while (true)
            {
                population = NextGeneration(population);

                int _max = MaxInPopulation(population);
                //generations.Add((population, _max));
                gens.Add(_max);
                if (currentMax < _max)
                {
                    currentMax = _max;
                    numberOfGensWithoutNewBest = 0;
                }
                if (currentMax == _max)
                {
                    numberOfGensWithoutNewBest++;
                }
                if (numberOfGensWithoutNewBest > MAX_GENERATIONS_WITHOUT_NEW_BEST)
                {
                    break;
                }

                if (_max == prevNum)
                {
                    repeatingNumber++;
                }
                else
                {
                    repeatingNumber = 0;
                }
                if (repeatingNumber > MAX_GENERATIONS_REPEATS)
                { 
                    break;
                }
                prevNum = _max;
            }
            //if (currentMax == 0)
            //{
            //    return Solve();
            //}
            return currentMax;
        }

        static void Main(string[] args)
        {
            string[] input = Console.ReadLine().Split(' ');
            maxBackpack = int.Parse(input[0]);
            numberOfItems = int.Parse(input[1]);
            for (int i = 0; i < numberOfItems; i++)
            {
                input = Console.ReadLine().Split(' ');
                items.Add((i, int.Parse(input[0]), int.Parse(input[1])));
            }

            var minItem = items.MinBy(x => x.Weight);
            if (minItem.Weight > maxBackpack)
            {
                Console.WriteLine(-1);
            }
            else
            {
                //Start stopwatch
                //Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();
                int _max = Solve();
                //Stop Stopwatch
                //stopWatch.Stop();
                PrintGenerations();

                //Console.WriteLine($"ANSWARE:{_max}");
                Console.WriteLine($"{_max}");
                //Print time
                //TimeSpan ts = stopWatch.Elapsed;
                //string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds,
                //        ts.Milliseconds / 10);
                //Console.WriteLine("RunTime: " + elapsedTime);
            }
        }

        private static void PrintGenerations()
        {

            //if (generations.Count > 10)
            //{
            //    Console.WriteLine("Gen0:" + generations[0].max);
            //    int N = (generations.Count - 1) / 8;
            //    for (int i = 0; i < 8; i++)
            //    {
            //        int k = i * N;
            //        Console.WriteLine($"Generation {k}: ");
            //        Console.WriteLine(generations[k].max);
            //        PrintPopulation(generations[k].Population);
            //    }
            //    Console.WriteLine($"Last Gen {generations.Count}:" + generations[generations.Count - 1].max);
            //}
            //else
            //{
            //    for (int i = 0; i < generations.Count; i++)
            //    {
            //        Console.WriteLine($"Generation {i}: ");
            //        Console.WriteLine(generations[i].max);
            //        PrintPopulation(generations[i].Population);
            //    }
            //}

            if (gens.Count > 10)
            {
                //Console.WriteLine("Gen0:" + gens[0]);
                Console.WriteLine(gens[0]);
                int N = (gens.Count - 1) / 8;
                for (int i = 0; i < 8; i++)
                {
                    int k = i * N;
                    //Console.WriteLine($"Generation {k}: ");
                    Console.WriteLine(gens[k]);
                }
                //Console.WriteLine($"Last Gen {gens.Count}:" + gens[gens.Count - 1]);
                Console.WriteLine(gens[gens.Count - 1]);

            }
            else
            {
                for (int i = 0; i < gens.Count; i++)
                {
                    //Console.WriteLine($"Generation {i}: ");
                    Console.WriteLine(gens[i]);
                }
            }

        }
    }
}