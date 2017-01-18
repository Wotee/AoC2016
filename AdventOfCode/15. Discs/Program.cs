using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using MathNet.Numerics;

namespace _15.Discs
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Regex regex = new Regex(@"Disc #\d has (\d{1,2}) positions; at time=0, it is at position (\d{1,2})");
            List<KeyValuePair<long, long>> discs = new List<KeyValuePair<long, long>>();
            using (FileStream fs = new FileStream("input.txt", FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Match match = regex.Match(line);
                    discs.Add(new KeyValuePair<long, long>(Convert.ToInt64(match.Groups[1].Value), Convert.ToInt64(match.Groups[2].Value)));
                }
            }
            for (int i = 0; i < discs.Count; i++)
            {
                discs[i] = new KeyValuePair<long, long>(discs[i].Key, discs[i].Value + (long)i + 1 < discs[i].Key ? discs[i].Value + (long)i + 1 : (discs[i].Value + (long)i + 1) % discs[i].Key);
            }

            Console.WriteLine(chineseRemainder(discs));

            discs.Add(new KeyValuePair<long, long>(11,0+1+6)); // Manually did the value + i + 1 trick
            Console.WriteLine(chineseRemainder(discs));
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalSeconds);
        }

        private static long chineseRemainder(List<KeyValuePair<long, long>> discs)
        {
            long p, prod = 1, sum = 0;

            for (int i = 0; i < discs.Count; i++)
                prod *= discs[i].Key;

            for (int i = 0; i < discs.Count; i++)
            {
                p = prod/discs[i].Key;
                sum += (discs[i].Key - discs[i].Value)*modInverse(p, discs[i].Key)*p;
            }
            return sum%prod;
        }

        private static long modInverse(long a, long m)
        {
            long x, y;
            greatestCommonDivisor(a, m, out x, out y);
            return (x%m + m)%m; // m is added to handle negative x
        }

        private static long greatestCommonDivisor(long a, long b, out long x, out long y)
        {
            // Base Case
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }

            long x1, y1; // To store results of recursive call
            long gcd = greatestCommonDivisor(b%a, a, out x1, out y1);
            x = y1 - (b/a)*x1;
            y = x1;
            return gcd;
        }
    }
}