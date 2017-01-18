using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace _14.OneTimePad
{
    public class Key
    {
        public string key;
        public int index;
        public int TTL;

        public Key(string key_, int index_, int TTL_ = 1000)
        {
            key = key_;
            index = index_;
            TTL = TTL_;
        }
    }

    class Program
    {   
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Dictionary<string, List<Key>> possibleKeysPhase1 = new Dictionary<string, List<Key>>();
            Dictionary<string, List<Key>> possibleKeysPhase2 = new Dictionary<string, List<Key>>();
            List<Key> confirmedKeysPhase1 = new List<Key>();
            List<Key> confirmedKeysPhase2 = new List<Key>();
            
            Regex possibleRegex = new Regex(@"(.)\1{2}");
            Regex confirmingRegex = new Regex(@"(.)\1{4}");
            bool phase1Found = false;
            bool phase2Found = false;
            string salt = args.Length != 0 ? args[0] : "jlmsuwbz"; // Puzzle input via paramter or hardcoded my default value
            Calculate phase1 = CalculateHashes;
            Calculate phase2 = CalculateHashes;

            for (int i = 0; i < int.MaxValue; i++)
            {
                Thread phase1Thread = new Thread(() => phase1Found = phase1(salt, i, 0, possibleRegex, confirmingRegex, possibleKeysPhase1, confirmedKeysPhase1));   
                Thread phase2Thread = new Thread(() => phase2Found = phase2(salt, i, 2016, possibleRegex, confirmingRegex, possibleKeysPhase2, confirmedKeysPhase2));
                phase2Thread.Priority = ThreadPriority.Highest;
                
                if (!phase1Found)
                {
                    phase1Thread.Start();
                }
                if (!phase2Found)
                {
                    phase2Thread.Start();
                }
                if (phase1Thread.IsAlive)
                    phase1Thread.Join();
                
                if (phase2Thread.IsAlive)
                    phase2Thread.Join();
                if (phase1Found && phase2Found)
                    break;
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalSeconds);
        }

        private delegate bool Calculate(string salt, int i, int stretch, Regex possibleRegex, Regex confirmingRegex, Dictionary<string, List<Key>> possibleKeys, List<Key> confirmedKeys);

        private static bool CalculateHashes(string salt, int i, int stretch, Regex possibleRegex, Regex confirmingRegex, Dictionary<string, List<Key>> possibleKeys, List<Key> confirmedKeys)
        {
            string pass = generatePassword(salt + i, stretch);
            Match possibleMatch = possibleRegex.Match(pass);
            Match confirmingMatch = confirmingRegex.Match(pass);

            if (confirmingMatch.Success)
            {
                foreach (Capture confirmingMatchCapture in confirmingMatch.Captures)
                {
                    string keyToConfirm = confirmingMatchCapture.Value.Substring(0, 3);
                    if (possibleKeys.ContainsKey(keyToConfirm))
                    {
                        foreach (Key key in possibleKeys[keyToConfirm])
                        {
                            confirmedKeys.Add(key);
                        }
                        possibleKeys.Remove(keyToConfirm);
                    }
                }
            }
            if (possibleMatch.Success)
            {
                if (!possibleKeys.ContainsKey(possibleMatch.Value))
                {
                    possibleKeys[possibleMatch.Value] = new List<Key>();
                }
                possibleKeys[possibleMatch.Value].Add(new Key(pass, i));
            }

            if (confirmedKeys.Count >= 64)
            {
                Console.WriteLine(String.Format("64th key produced by index: {0} in phase {1}", confirmedKeys[63].index, stretch == 0 ? "1" : "2"));
                return true;
            }
            ReduceTTL(possibleKeys);
            return false;
        }

        private static void ReduceTTL(Dictionary<string,List<Key>> possibleKeys )
        {
            foreach (var keys in possibleKeys)
            {
                for (int i = 0; i < keys.Value.Count; i++ )
                {
                    keys.Value[i].TTL--;
                    if (keys.Value[i].TTL == 0)
                    {
                        keys.Value.Remove(keys.Value[i]);
                        i--;
                    }
                }
            }
        }

        public static string generatePassword(string input, int stretch)
        {
            string stringToHash = input;
            for (int j = 0; j <= stretch; j++)
            {
                using (MD5 md5hash = MD5.Create())
                {
                    byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    stringToHash = sBuilder.ToString();
                }
            }
            return stringToHash;
        }
    }
}
