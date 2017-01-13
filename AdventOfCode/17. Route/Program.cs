using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace _17.Route
{
    class Program
    {
        static List<KeyValuePair<string, KeyValuePair<int, int>>> possibleRoutes = new List<KeyValuePair<string, KeyValuePair<int, int>>>();
        static string[] directions = new[] { "U", "D", "L", "R" };
        
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string input = args.Length != 0 ? args[0] : "yjjvjgan"; // Puzzle input via paramter or hardcoded my default value
            string shortestPath = "";
            int longestPathLength = 0;
            possibleRoutes.Add(new KeyValuePair<string, KeyValuePair<int,int>>(input, new KeyValuePair<int, int>(0,0)));
            for(int i = 0; i < possibleRoutes.Count; i++)
            {
                bool[] doors = CheckDoors(possibleRoutes[i].Key);
                for (int j = 0; j < doors.Length; j++)
                {
                    if (doors[j] && IsDoor(possibleRoutes[i].Value, j))
                    {
                        KeyValuePair<int, int> location = possibleRoutes[i].Value;
                        if (NewLocation(directions[j], ref location))
                        {
                            if(String.IsNullOrEmpty(shortestPath))
                                shortestPath = (possibleRoutes[i].Key + directions[j]).Substring(input.Length);
                            longestPathLength = possibleRoutes[i].Key.Length - input.Length + 1;
                            continue;
                        }
                        possibleRoutes.Add(new KeyValuePair<string, KeyValuePair<int, int>>(possibleRoutes[i].Key + directions[j], location));
                    }
                }
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalSeconds);
            Console.WriteLine("Shortest path: " + shortestPath);
            Console.WriteLine("Longest path length: " + longestPathLength);
        }

        private static bool IsDoor(KeyValuePair<int, int> pair, int i)
        {
            switch (i)
            {
                case 0:
                    return pair.Key > 0;
                case 1:
                    return pair.Key < 3;
                case 2:
                    return pair.Value > 0;
                case 3:
                    return pair.Value < 3;
                default:
                    return false;
            }
        }

        private static bool NewLocation(string direction, ref KeyValuePair<int, int> location)
        {
            switch (direction)
            {
                case "U":
                    location = new KeyValuePair<int, int>(location.Key - 1, location.Value);
                    break;
                case "D":
                    location = new KeyValuePair<int, int>(location.Key + 1, location.Value);
                    break;
                case "L":
                    location = new KeyValuePair<int, int>(location.Key, location.Value - 1);
                    break;
                case "R":
                    location = new KeyValuePair<int, int>(location.Key, location.Value + 1);
                    break;
            }
            if (location.Key == 3 && location.Value == 3)
                return true;
            return false;
        }

        public static bool[] CheckDoors(string input)
        {
            bool[] returnValue = new bool[4];
            Regex regex = new Regex(@"[bcdef]");
            using (MD5 md5hash = MD5.Create())
            {
                byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                    returnValue[i] = regex.IsMatch(sBuilder[i].ToString());
                }
                return returnValue;
            }
        }
    }
}
