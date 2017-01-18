using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _8.Decompression
{
    interface IParser
    {
        long length(StreamReader sr);
    }

    class FirstParser : IParser
    {
        public long length(StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                string newline = "";
                string line = sr.ReadLine();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line.ElementAt(i) == '(')
                    {
                        int next = line.IndexOf(')', i) + 1;
                        string instruction = line.Substring(i, next - i);
                        int length = Int32.Parse(instruction.Substring(1, instruction.IndexOf('x') - 1));
                        int amount = Int32.Parse(instruction.Substring(instruction.IndexOf('x') + 1, instruction.Length - instruction.IndexOf('x') - 2));
                        string copyThis = line.Substring(i + instruction.Length, length);
                        for (int j = 0; j < amount; j++)
                            newline += copyThis;
                        i = next + length - 1;
                    }
                    else
                    {
                        newline += line.ElementAt(i);
                    }
                }
                return newline.Length;
            }
            return 0;
        }
    }

    class SecondParser : IParser
    {
        public long length(StreamReader sr)
        {
            long totalLength = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                totalLength += OpenRecursions(line);
            }
            return totalLength;
        }

        private long OpenRecursions(string input)
        {
            if (!input.Contains('('))
                return input.Length;
            long recursionLength = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input.ElementAt(i) == '(')
                {
                    int next = input.IndexOf(')', i) + 1;
                    string instruction = input.Substring(i, next - i);
                    int length = Int32.Parse(instruction.Substring(1, instruction.IndexOf('x') - 1));
                    int amount = Int32.Parse(instruction.Substring(instruction.IndexOf('x') + 1, instruction.Length - instruction.IndexOf('x') - 2));
                    recursionLength += amount * OpenRecursions(input.Substring(i + instruction.Length, length));
                    i = next + length - 1;
                }
                else
                {
                    recursionLength++;
                }
            }
            return recursionLength;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                IParser parser;
                for (int phase = 1; phase <= 2; phase++)
                {
                    switch (phase)
                    {
                        case 1:
                            parser = new FirstParser();
                            break;
                        case 2:
                            parser = new SecondParser();
                            break;
                        default:
                            Console.WriteLine("Unknown parser. Aborting");
                            continue;
                    }
                    Console.WriteLine("Length of phase " + phase + ": " + parser.length(sr));
                    sr.DiscardBufferedData();
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                }
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalSeconds);
        }
    }
}
