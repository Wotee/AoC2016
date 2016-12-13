using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace _10.Bots
{
    public abstract class Interface
    {
        public short ID;
        public List<short> values;
    }

    public class Bot : Interface
    {
        public Interface lower;     
        public Interface higher;

        public Bot(short id)
        {
            ID = id;
            lower = null;
            higher = null;
            values = new List<short>();
        }
    }

    class Output : Interface
    {        
        public Output(short id)
        {
            ID = id;
            values = new List<short>();
        }
    }

    class Program
    {
        private static Bot[] bots;
        private static Output[] outputs;

        static void Main(string[] args)
        {
            var regex = new Regex(@"value (?<value>\d+) goes to bot (?<bot>\d+)|bot (?<from>\d+) gives low to (?<lower>(bot|output)) (?<lowvalue>\d+) and high to (?<higher>(bot|output)) (?<highvalue>\d+)");            
            bots = new Bot[210];
            outputs = new Output[21];
            for (short i = 0; i < bots.Length; i++)
            {
                bots[i] = new Bot(i);
            }
            for (short i = 0; i < outputs.Length; i++)
            {
                outputs[i] = new Output(i);
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
                            bots[short.Parse(match.Groups["bot"].Value)].values.Add(short.Parse(match.Groups["value"].Value));
                        }
                        else if (match.Groups["from"].Success)
                        {
                            if(match.Groups["lower"].Value == "bot")
                            {
                                bots[short.Parse(match.Groups["from"].Value)].lower = bots[short.Parse(match.Groups["lowvalue"].Value)];
                            }
                            else
                            {
                                bots[short.Parse(match.Groups["from"].Value)].lower = outputs[short.Parse(match.Groups["lowvalue"].Value)];
                            }
                            if (match.Groups["higher"].Value == "bot")
                            {
                                bots[short.Parse(match.Groups["from"].Value)].higher = bots[short.Parse(match.Groups["highvalue"].Value)];
                            }
                            else
                            {
                                bots[short.Parse(match.Groups["from"].Value)].higher = outputs[short.Parse(match.Groups["highvalue"].Value)];
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
            bool botFound = false;
            foreach (Bot bot in bots)
            {
                bot.MoveChips(bots);
            }
            // Asnwer for part 2
            long answer = 1;
            for(int i = 0; i <= 2; i++)
            {
                Output output = outputs[i];
                foreach (short value in output.values)
                {
                    answer *= value;
                }
            }
            Console.WriteLine("Phase 2: " + answer);
        }
    }

    public static class ExtensionMethods
    {
        public static void MoveChips(this Bot bot, Bot[] bots)
        {
            // Answer for part 1
            if (bot.values.Contains(17) && bot.values.Contains(61))
                Console.WriteLine("Phase 1: " + bot.ID);

            if (bot.values.Count == 2)
            {
                bot.higher.values.Add(bot.values.Max());
                if (bot.higher.GetType() == typeof(Bot))
                {
                    Bot bot2 = (Bot)bot.higher;
                    bot2.MoveChips(bots);
                }
                bot.lower.values.Add(bot.values.Min());
                if (bot.lower.GetType() == typeof(Bot))
                {
                    Bot bot2 = (Bot)bot.lower;
                    bot2.MoveChips(bots);
                }
                bot.values.Clear();
            }
        }
    }
}
