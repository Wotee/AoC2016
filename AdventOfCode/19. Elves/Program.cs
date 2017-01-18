using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _19.Elves
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int input = args.Length > 0 ? Convert.ToInt32(args[0]) : 3018458;
            // The Josephus problem can be used to solve the case.
            // One way of solveing it, is moving hishgest order 1-bit, to the end of the bits
            // For example 110010 becomes 100101 etc.
            // Following code does left shift by one, then turns the highest order 1-bit to 0 and adds 1.
            Console.WriteLine((input << 1) - (int)Math.Pow(2, Convert.ToString(input << 1, 2).Length - 1) | 1);
            
            // Initialize part two
            Queue<int> left = new Queue<int>();
            Queue<int> right = new Queue<int>();
            for (int i = 1; i <= input; i++)
            {
                if (i <= input/2)
                    left.Enqueue(i);
                else
                    right.Enqueue(i);
            }
            
            // Go!
            while (left.Count != 0)
            {
                right.Dequeue();
                right.Enqueue(left.Dequeue());
                // Balance the queues
                if(((left.Count + right.Count) & 1) != 1)
                {
                    left.Enqueue(right.Dequeue());
                }
            }
            Console.WriteLine(right.First());
            watch.Stop();            
            Console.WriteLine(watch.Elapsed.TotalSeconds);
        }
    }
}