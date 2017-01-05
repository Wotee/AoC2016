using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
            Dictionary<string, List<Key>> possibleKeysPhase1 = new Dictionary<string, List<Key>>();
            Dictionary<string, List<Key>> possibleKeysPhase2 = new Dictionary<string, List<Key>>();
            List<Key> confirmedKeysPhase1 = new List<Key>();
            List<Key> confirmedKeysPhase2 = new List<Key>();
            
            Regex possibleRegex = new Regex(@"(.)\1{2}");
            Regex confirmingRegex = new Regex(@"(.)\1{4}");
            bool phase1Found = false;
            string salt = args.Length != 0 ? args[0] : "jlmsuwbz"; // Puzzle input via paramter or hardcoded my default value
            for (int i = 0; i < int.MaxValue; i++)
            {

                string pass = generatePassword(salt + i, 2016);
                Match possibleMatch = possibleRegex.Match(pass);
                Match confirmingMatch = confirmingRegex.Match(pass);
                                
                if(confirmingMatch.Success)
                {
                    foreach (Capture confirmingMatchCapture in confirmingMatch.Captures)
                    {
                        string keyToConfirm = confirmingMatchCapture.Value.Substring(0, 3);
                        if (possibleKeysPhase1.ContainsKey(keyToConfirm))
                        {
                            foreach (Key key in possibleKeysPhase1[keyToConfirm])
                            {
                                confirmedKeysPhase1.Add(key);
                            }
                            possibleKeysPhase1.Remove(keyToConfirm);
                        }
                    }
                }
                if (possibleMatch.Success)
                {
                    if (!possibleKeysPhase1.ContainsKey(possibleMatch.Value))
                    {
                        possibleKeysPhase1[possibleMatch.Value] = new List<Key>();
                    }
                    possibleKeysPhase1[possibleMatch.Value].Add(new Key(pass, i));
                }
                
                if (confirmedKeysPhase1.Count >= 64)
                {
                    Console.WriteLine("64th key produced by index: " + confirmedKeysPhase1[63].index);
                    break;
                }
                ReduceTTL(possibleKeysPhase1);
            }
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
