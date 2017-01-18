using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

//  V.Tarvus
//  4.12.2016
//
//  Link to the problem:  http://adventofcode.com/2016/day/4
//
//  Errorhandling is not implemented, 
//  since given input is clean, and
//  this program does not need to handle
//  user input..

namespace Room_IDs
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                int sumOfValidIDs = 0;
                List<string> validNames = new List<string>();
                while (!sr.EndOfStream)
                {
                    string roomHash = sr.ReadLine();
                    int startChecksum = roomHash.LastIndexOf('[') + 1;
                    int endChecksum = roomHash.LastIndexOf(']');
                    string checksum = roomHash.Substring(startChecksum, endChecksum - startChecksum);
                    var match = Regex.Match(roomHash, @"[0-9]+");
                    string encryptedName = roomHash.Substring(0, match.Index);
                    Dictionary<char,int> characters = new Dictionary<char, int>();
                  
                    foreach (char c in encryptedName)
                    {
                        if (c == '-') continue;
                        if (characters.ContainsKey(c))
                        {
                            characters[c]++;
                        }
                        else
                        {
                            characters.Add(c, 1);
                        }
                    }
                    string calculatedChecksum = new string(
                        characters.ToList()
                        .OrderByDescending(x => x.Value)
                        .ThenBy(y => y.Key)
                        .Select(z => z.Key)
                        .Take(5)
                        .ToArray());
                    if (calculatedChecksum == checksum)
                    {
                        int id = Int32.Parse(match.Value);
                        sumOfValidIDs += id;
                        int AddTo = id % 26;
                        char[] Name = new char[encryptedName.Length];
                        for (int i = 0; i < encryptedName.Length; i++)
                        {
                            if (encryptedName[i] == '-')
                                Name[i] = ' ';
                            else
                            {
                                int value = encryptedName[i] + AddTo;
                                Name[i] = (char) ((value > 122) ? value - 26 : value);
                            }
                        }
                        string realName = new string(Name);
                        validNames.Add(realName);
                        if (realName.Contains("northpole object storage"))
                            Console.WriteLine("North Pole Objects Storage ID: " + id);
                    }
                }
                Console.WriteLine("Sum om valid IDs: " + sumOfValidIDs);
                watch.Stop();
                Console.WriteLine(watch.Elapsed.TotalSeconds);
            }
        }
    }
}