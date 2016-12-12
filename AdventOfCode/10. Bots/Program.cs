using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace _10.Bots
{
    abstract class Interface
    {
        public Int16 ID;
    }

    class Bot : Interface
    {
        new public Int16 ID;
        public Int16[] chips;
        public Interface lower;     
        public Interface higher;

        public Bot(Int16 id)
        {
            ID = id;
            lower = null;
            higher = null;
            chips = new Int16[2] {Int16.MaxValue, Int16.MaxValue};
        }
    }

    class Output : Interface
    {
        new public Int16 ID;
        public List<int> values;

        public Output(Int16 id)
        {
            ID = id;
            values = new List<int>();
        }
    }

    class Program
    {
        private static Bot[] bots;
        private static List<int>[] output;

        static void Main(string[] args)
        {
            bots = new Bot[210];
            for (Int16 i = 0; i < bots.Length; i++)
            {
                bots[i] = new Bot(i);
            }
            Regex rgx = new Regex(@"(bot|value|output) [0-9]+");
            Regex rgx2 = new Regex(@"[0-9]+");
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    var matches = rgx.Matches(line);
                    
                    if (line.StartsWith("bot"))
                    {
                        // Bot value
                        string botMatch = rgx2.Match(matches[0].Value).Value;
                        Int16 bot = Int16.Parse(botMatch);

                        // Low value
                        if (matches[1].Value.Contains("bot"))
                        {
                            string match = rgx2.Match(matches[1].Value).Value;
                            bots[bot].lower = bots[Int16.Parse(match)];
                        }
                        else
                        {
                            string match = rgx2.Match(matches[1].Value).Value;
                            bots[bot].lower = new Output(Int16.Parse(match));
                        }

                        // High value
                        if (matches[2].Value.Contains("bot"))
                        {
                            string match = rgx2.Match(matches[2].Value).Value;
                            bots[bot].higher = bots[(Int16.Parse(match))];
                        }
                        else
                        {
                            string match = rgx2.Match(matches[2].Value).Value;
                            bots[bot].higher = new Output(Int16.Parse(match));
                        }
                        bots[bot].CheckStatus(bots);
                    }
                    else if (line.StartsWith("value"))
                    {
                        Int16 value = Int16.Parse(rgx2.Match(matches[0].Value).Value);
                        Int16 bot = Int16.Parse(rgx2.Match(matches[1].Value).Value);
                        bots[bot].Add(value, bots);
                    }
                    else
                    {
                        Console.WriteLine("Weird line...");
                    }
                }
            }
        }
    }

    static class ExtensionMethods
    {
        internal static void Add(this Interface addTo, Int16 value, Bot[] bots)
        {
            if (addTo.GetType() == typeof(Bot))
            {
                Bot bot = (Bot) addTo;
                if (bot.chips[0].Equals(Int16.MaxValue))
                    bot.chips[0] = value;
                else if (bot.chips[1].Equals(Int16.MaxValue))
                    bot.chips[1] = value;
                else
                    throw new Exception("Full chips..");
                bot.SortChips();
                if (bot.chips[0] == 17 && bot.chips[1] == 61)
                    Console.WriteLine("Bot: " + bot.ID);
                bot.CheckStatus(bots);
            }
            else
            {
                Output output = (Output) addTo;
                output.values.Add(value);
            }
        }

        internal static void CheckStatus(this Bot bot, Bot[] bots)
        {
            if (bot.higher != null && bot.lower != null && bot.chips[0] != Int16.MaxValue && bot.chips[1] != Int16.MaxValue)
                bot.Move(bots);
        }

        internal static void Move(this Bot bot, Bot[] bots)
        {
            bot.lower.Add(bot.chips[0], bots);
            bot.higher.Add(bot.chips[0], bots);
            bot.chips[0] = Int16.MaxValue;
            bot.chips[0] = Int16.MaxValue;
        }

        internal static void SortChips(this Bot bot)
        {
            if (bot.chips[0] > bot.chips[1])
            {
                bot.chips[0] ^= bot.chips[1];
                bot.chips[1] ^= bot.chips[0];
                bot.chips[0] ^= bot.chips[1];
            }
        }
    }
}
