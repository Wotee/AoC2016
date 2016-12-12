using System;
using System.Collections.Generic;
using System.IO;

//  V.Tarvus
//  3.12.2016
//
//  Link to the problem:  http://adventofcode.com/2016/day/3
//
//  Errorhandling is not implemented, 
//  since given input is clean, and
//  this program does not need to handle
//  user input..

namespace Triangles
{
    interface IParser
    {
        int Parse(StreamReader input, ref int size);
    }

    class RowParser : IParser
    {
        public int Parse(StreamReader sr, ref int size)
        {
            int antiTriangles = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                int a = Int32.Parse(line.Substring(0, 5).TrimStart(' '));
                int b = Int32.Parse(line.Substring(6, 5).TrimStart(' '));
                int c = Int32.Parse(line.Substring(11).TrimStart(' '));
                int[] sides = { a, b, c };
                Array.Sort(sides);
                if (sides[2] >= sides[1] + sides[0])
                    ++antiTriangles;
            }
            return antiTriangles;
        }
    }

    class ColumnParser : IParser
    {
        public int Parse(StreamReader sr, ref int size)
        {
            int antiTriangles = 0;
            while (!sr.EndOfStream)
            {
                string[] lines = { sr.ReadLine(), sr.ReadLine(), sr.ReadLine() };
                List<int> listSides1 = new List<int>();
                List<int> listSides2 = new List<int>();
                List<int> listSides3 = new List<int>();
                foreach (string line in lines)
                {
                    if (!String.IsNullOrEmpty(line)) ++size;
                    listSides1.Add(Int32.Parse(line.Substring(0, 5).TrimStart(' ')));
                    listSides2.Add(Int32.Parse(line.Substring(6, 5).TrimStart(' ')));
                    listSides3.Add(Int32.Parse(line.Substring(11).TrimStart(' ')));
                }
                int[][] allSides = { listSides1.ToArray(), listSides2.ToArray(), listSides3.ToArray() };
                foreach (int[] sides in allSides)
                {
                    Array.Sort(sides);
                    if (sides[2] >= sides[1] + sides[0])
                        ++antiTriangles;
                }
            }
            return antiTriangles;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                for (int phase = 1; phase <= 2; phase++)
                {
                    IParser parser;
                    switch (phase)
                    {
                        case 1:
                            parser = new RowParser();
                            break;
                        case 2:
                            parser = new ColumnParser();
                            break;
                        default:
                            throw new Exception("Unknown parser");
                    }

                    int size = 0;
                    int antiTriangles = 0;
                    using (StreamReader sr = new StreamReader("input.txt"))
                    {
                        antiTriangles = parser.Parse(sr, ref size);
                    }
                    Console.WriteLine("In phase" + phase + ", " + (size - antiTriangles) + " are triangles");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unknown parser.. Aborting..");
            }
        }
    }
}
