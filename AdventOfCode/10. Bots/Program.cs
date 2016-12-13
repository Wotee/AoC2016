using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace _10.Bots
{
    abstract class Interface
    {
        public Int16 ID;
        public List<int> values;
    }

    class Bot : Interface
    {
        public Interface lower;     
        public Interface higher;

        public Bot(Int16 id)
        {
            ID = id;
            lower = null;
            higher = null;
        }
    }

    class Output : Interface
    {        
        public Output(Int16 id)
        {
            ID = id;
            values = new List<int>();
        }
    }

    class Program
    {
        private static Bot[] bots;

        static void Main(string[] args)
        {
            var regex = new Regex(@"value (?<value>\d+) goes to bot (?<bot>\d+)|bot (?<from>\d+) gives low to (?<lower>(bot|output)) (?<lowvalue>\d+) and high to (?<higher>(bot|output)) (?<highvalue>\d+)");            
            bots = new Bot[210];
            for (Int16 i = 0; i < bots.Length; i++)
            {
                bots[i] = new Bot(i);
            }

            // Initialize situation
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string line = sr.ReadLine();
                        var match = regex.Match(line);
                        // Add values
                        if (match.Groups["value"].Success)
                        {
                            bots[short.Parse(match.Groups["from"].Value)].values.Add(short.Parse(match.Groups["value"].Value));
                        }
                        else if (match.Groups["from"].Success)
                        {
                            if(match.Groups["lower"].Value == "bot")
                            {
                                bots[short.Parse(match.Groups["bot"].Value)].lower = bots[short.Parse(match.Groups["lowvalue"].Value)];
                            }
                            else
                            {
                                bots[short.Parse(match.Groups["bot"].Value)].lower = new Output(short.Parse(match.Groups["lowvalue"].Value));
                            }
                            if (match.Groups["higher"].Value == "bot")
                            {
                                bots[short.Parse(match.Groups["bot"].Value)].higher = bots[short.Parse(match.Groups["highvalue"].Value)];
                            }
                            else
                            {
                                bots[short.Parse(match.Groups["bot"].Value)].higher = new Output(short.Parse(match.Groups["highvalue"].Value));
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Bad input line: " + ex.Message);
                    }
                }
            }       
        }
    }
}
