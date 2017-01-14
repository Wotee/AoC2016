using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace _18.Tiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            char[] input = args.Length != 0 ? args[0].ToCharArray() : ".^^^.^.^^^^^..^^^..^..^..^^..^.^.^.^^.^^....^.^...^.^^.^^.^^..^^..^.^..^^^.^^...^...^^....^^.^^^^^^^".ToCharArray(); // Puzzle input via paramter or hardcoded my default value
            int safeTiles = 0;
            for (int i = 0; i < 400000; i++)
            {
                if (i == 40)
                    Console.WriteLine(safeTiles);
                char[] nextLine = new char[input.Length];
                for (int j = 0; j < input.Length; j++)
                {
                    if (input[j] == '.')
                        safeTiles++;
                    bool leftTrap = j == 0 ? false : input[j - 1] == '^';
                    bool rightTrap = j == input.Length - 1 ? false : input[j + 1] == '^';
                    nextLine[j] = leftTrap ^ rightTrap ? '^' : '.';
                }
                input = nextLine;
            }
            Console.WriteLine(safeTiles);
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalSeconds);
        }
    }
}
