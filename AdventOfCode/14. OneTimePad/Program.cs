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

		public static byte[] GetStartOfFingerprint(string input)
		{
			uint A = 0x67452301;
			uint B = 0xEFCDAB89;
			uint C = 0x98BADCFE;
			uint D = 0X10325476;
			uint[] X = new uint[16];
			byte[] byteInput = new byte[input.Length];
			for (int i = 0; i < input.Length; i++)
				byteInput[i] = (byte)input[i];

			byte[] bMsg; //buffer to hold bits

			// create a buffer with bits padded and length is also padded
			uint pad; //no of padding bits for 448 mod 512 
			ulong sizeMsg; //64 bit size pad
			uint sizeMsgBuff; //buffer size in multiple of bytes
			int temp = (448 - ((byteInput.Length * 8) % 512)); //temporary 

			// Get the size of pad
			pad = (uint)((temp + 512) % 512);
			if (pad == 0)
				pad = 512;

			sizeMsgBuff = (uint)((byteInput.Length) + (pad / 8) + 8);
			sizeMsg = (ulong)byteInput.Length * 8;
			bMsg = new byte[sizeMsgBuff];

			for (int i = 0; i < byteInput.Length; i++)
				bMsg[i] = byteInput[i];

			// making first bit of padding 1,
			bMsg[byteInput.Length] |= 0x80;

			//writing the size value
			for (int i = 8; i > 0; i--)
				bMsg[sizeMsgBuff - i] = (byte)(sizeMsg >> ((8 - i) * 8) & 0x00000000000000ff);

			uint N = (uint)(bMsg.Length * 8) / 32; //no of 32 bit blocks

			for (uint i = 0; i < N / 16; i++)
			{
				uint block = i << 6;
				for (uint j = 0; j < 61; j += 4)
					X[j >> 2] = (uint)(((bMsg[block + (j + 3)]) << 24) | ((bMsg[block + (j + 2)]) << 16) | ((bMsg[block + (j + 1)]) << 8) | ((bMsg[block + (j)])));

				uint AA = A, BB = B, CC = C, DD = D;
				// Round 1
				uint perfTemp = (A + ((B & C) | (~(B) & D)) + X[0] + T[0]);
				A = B + ((perfTemp >> 25) | (perfTemp << 7));
				perfTemp = (D + ((A & B) | (~(A) & C)) + X[1] + T[1]);
				D = A + ((perfTemp >> 20) | (perfTemp << 12));
				perfTemp = (C + ((D & A) | (~(D) & B)) + X[2] + T[2]);
				C = D + ((perfTemp >> 15) | (perfTemp << 17));
				perfTemp = (B + ((C & D) | (~(C) & A)) + X[3] + T[3]);
				B = C + ((perfTemp >> 10) | (perfTemp << 22));
				perfTemp = (A + ((B & C) | (~(B) & D)) + X[4] + T[4]);
				A = B + ((perfTemp >> 25) | (perfTemp << 7));
				perfTemp = (D + ((A & B) | (~(A) & C)) + X[5] + T[5]);
				D = A + ((perfTemp >> 20) | (perfTemp << 12));
				perfTemp = (C + ((D & A) | (~(D) & B)) + X[6] + T[6]);
				C = D + ((perfTemp >> 15) | (perfTemp << 17));
				perfTemp = (B + ((C & D) | (~(C) & A)) + X[7] + T[7]);
				B = C + ((perfTemp >> 10) | (perfTemp << 22));
				perfTemp = (A + ((B & C) | (~(B) & D)) + X[8] + T[8]);
				A = B + ((perfTemp >> 25) | (perfTemp << 7));
				perfTemp = (D + ((A & B) | (~(A) & C)) + X[9] + T[9]);
				D = A + ((perfTemp >> 20) | (perfTemp << 12));
				perfTemp = (C + ((D & A) | (~(D) & B)) + X[10] + T[10]);
				C = D + ((perfTemp >> 15) | (perfTemp << 17));
				perfTemp = (B + ((C & D) | (~(C) & A)) + X[11] + T[11]);
				B = C + ((perfTemp >> 10) | (perfTemp << 22));
				perfTemp = (A + ((B & C) | (~(B) & D)) + X[12] + T[12]);
				A = B + ((perfTemp >> 25) | (perfTemp << 7));
				perfTemp = (D + ((A & B) | (~(A) & C)) + X[13] + T[13]);
				D = A + ((perfTemp >> 20) | (perfTemp << 12));
				perfTemp = (C + ((D & A) | (~(D) & B)) + X[14] + T[14]);
				C = D + ((perfTemp >> 15) | (perfTemp << 17));
				perfTemp = (B + ((C & D) | (~(C) & A)) + X[15] + T[15]);
				B = C + ((perfTemp >> 10) | (perfTemp << 22));
				// Round 2
				perfTemp = (A + ((B & D) | (C & ~D)) + X[1] + T[16]);
				A = B + ((perfTemp >> 27) | (perfTemp << 5));
				perfTemp = (D + ((A & C) | (B & ~C)) + X[6] + T[17]);
				D = A + ((perfTemp >> 23) | (perfTemp << 9));
				perfTemp = (C + ((D & B) | (A & ~B)) + X[11] + T[18]);
				C = D + ((perfTemp >> 18) | (perfTemp << 14));
				perfTemp = (B + ((C & A) | (D & ~A)) + X[0] + T[19]);
				B = C + ((perfTemp >> 12) | (perfTemp << 20));
				perfTemp = (A + ((B & D) | (C & ~D)) + X[5] + T[20]);
				A = B + ((perfTemp >> 27) | (perfTemp << 5));
				perfTemp = (D + ((A & C) | (B & ~C)) + X[10] + T[21]);
				D = A + ((perfTemp >> 23) | (perfTemp << 9));
				perfTemp = (C + ((D & B) | (A & ~B)) + X[15] + T[22]);
				C = D + ((perfTemp >> 18) | (perfTemp << 14));
				perfTemp = (B + ((C & A) | (D & ~A)) + X[4] + T[23]);
				B = C + ((perfTemp >> 12) | (perfTemp << 20));
				perfTemp = (A + ((B & D) | (C & ~D)) + X[9] + T[24]);
				A = B + ((perfTemp >> 27) | (perfTemp << 5));
				perfTemp = (D + ((A & C) | (B & ~C)) + X[14] + T[25]);
				D = A + ((perfTemp >> 23) | (perfTemp << 9));
				perfTemp = (C + ((D & B) | (A & ~B)) + X[3] + T[26]);
				C = D + ((perfTemp >> 18) | (perfTemp << 14));
				perfTemp = (B + ((C & A) | (D & ~A)) + X[8] + T[27]);
				B = C + ((perfTemp >> 12) | (perfTemp << 20));
				perfTemp = (A + ((B & D) | (C & ~D)) + X[13] + T[28]);
				A = B + ((perfTemp >> 27) | (perfTemp << 5));
				perfTemp = (D + ((A & C) | (B & ~C)) + X[2] + T[29]);
				D = A + ((perfTemp >> 23) | (perfTemp << 9));
				perfTemp = (C + ((D & B) | (A & ~B)) + X[7] + T[30]);
				C = D + ((perfTemp >> 18) | (perfTemp << 14));
				perfTemp = (B + ((C & A) | (D & ~A)) + X[12] + T[31]);
				B = C + ((perfTemp >> 12) | (perfTemp << 20));
				// Round 3
				perfTemp = (A + (B ^ C ^ D) + X[5] + T[32]);
				A = B + ((perfTemp >> 28) | (perfTemp << 4));
				perfTemp = (D + (A ^ B ^ C) + X[8] + T[33]);
				D = A + ((perfTemp >> 21) | (perfTemp << 11));
				perfTemp = (C + (D ^ A ^ B) + X[11] + T[34]);
				C = D + ((perfTemp >> 16) | (perfTemp << 16));
				perfTemp = (B + (C ^ D ^ A) + X[14] + T[35]);
				B = C + ((perfTemp >> 9) | (perfTemp << 23));
				perfTemp = (A + (B ^ C ^ D) + X[1] + T[36]);
				A = B + ((perfTemp >> 28) | (perfTemp << 4));
				perfTemp = (D + (A ^ B ^ C) + X[4] + T[37]);
				D = A + ((perfTemp >> 21) | (perfTemp << 11));
				perfTemp = (C + (D ^ A ^ B) + X[7] + T[38]);
				C = D + ((perfTemp >> 16) | (perfTemp << 16));
				perfTemp = (B + (C ^ D ^ A) + X[10] + T[39]);
				B = C + ((perfTemp >> 9) | (perfTemp << 23));
				perfTemp = (A + (B ^ C ^ D) + X[13] + T[40]);
				A = B + ((perfTemp >> 28) | (perfTemp << 4));
				perfTemp = (D + (A ^ B ^ C) + X[0] + T[41]);
				D = A + ((perfTemp >> 21) | (perfTemp << 11));
				perfTemp = (C + (D ^ A ^ B) + X[3] + T[42]);
				C = D + ((perfTemp >> 16) | (perfTemp << 16));
				perfTemp = (B + (C ^ D ^ A) + X[6] + T[43]);
				B = C + ((perfTemp >> 9) | (perfTemp << 23));
				perfTemp = (A + (B ^ C ^ D) + X[9] + T[44]);
				A = B + ((perfTemp >> 28) | (perfTemp << 4));
				perfTemp = (D + (A ^ B ^ C) + X[12] + T[45]);
				D = A + ((perfTemp >> 21) | (perfTemp << 11));
				perfTemp = (C + (D ^ A ^ B) + X[15] + T[46]);
				C = D + ((perfTemp >> 16) | (perfTemp << 16));
				perfTemp = (B + (C ^ D ^ A) + X[2] + T[47]);
				B = C + ((perfTemp >> 9) | (perfTemp << 23));
				// Round 4
				perfTemp = (A + (C ^ (B | ~D)) + X[0] + T[48]);
				A = B + ((perfTemp >> 26) | (perfTemp << 6));
				perfTemp = (D + (B ^ (A | ~C)) + X[7] + T[49]);
				D = A + ((perfTemp >> 22) | (perfTemp << 10));
				perfTemp = (C + (A ^ (D | ~B)) + X[14] + T[50]);
				C = D + ((perfTemp >> 17) | (perfTemp << 15));
				perfTemp = (B + (D ^ (C | ~A)) + X[5] + T[51]);
				B = C + ((perfTemp >> 11) | (perfTemp << 21));
				perfTemp = (A + (C ^ (B | ~D)) + X[12] + T[52]);
				A = B + ((perfTemp >> 26) | (perfTemp << 6));
				perfTemp = (D + (B ^ (A | ~C)) + X[3] + T[53]);
				D = A + ((perfTemp >> 22) | (perfTemp << 10));
				perfTemp = (C + (A ^ (D | ~B)) + X[10] + T[54]);
				C = D + ((perfTemp >> 17) | (perfTemp << 15));
				perfTemp = (B + (D ^ (C | ~A)) + X[1] + T[55]);
				B = C + ((perfTemp >> 11) | (perfTemp << 21));
				perfTemp = (A + (C ^ (B | ~D)) + X[8] + T[56]);
				A = B + ((perfTemp >> 26) | (perfTemp << 6));
				perfTemp = (D + (B ^ (A | ~C)) + X[15] + T[57]);
				D = A + ((perfTemp >> 22) | (perfTemp << 10));
				perfTemp = (C + (A ^ (D | ~B)) + X[6] + T[58]);
				C = D + ((perfTemp >> 17) | (perfTemp << 15));
				perfTemp = (B + (D ^ (C | ~A)) + X[13] + T[59]);
				B = C + ((perfTemp >> 11) | (perfTemp << 21));
				perfTemp = (A + (C ^ (B | ~D)) + X[4] + T[60]);
				A = B + ((perfTemp >> 26) | (perfTemp << 6));
				//D = A + (((D + (B ^ (A | ~C)) + X[11] + T[61]) >> 22) | ((D + (B ^ (A | ~C)) + X[11] + T[61]) << 10));
				//C = D + (((C + (A ^ (D | ~B)) + X[2] + T[62]) >> 17) | ((C + (A ^ (D | ~B)) + X[2] + T[62]) << 15));
				//B = C + (((B + (D ^ (C | ~A)) + X[9] + T[63]) >> 11) | ((B + (D ^ (C | ~A)) + X[9] + T[63]) << 21));

				A += AA;
				//B += BB;
				//C += CC;
				//D += DD;
			}
			return BitConverter.GetBytes(A);
			//return ReverseByte(A).ToString("X8") + ReverseByte(B).ToString("X8") + ReverseByte(C).ToString("X8") + ReverseByte(D).ToString("X8");
		}
		//public static uint ReverseByte(uint uiNumber)
		//{
		//	return (((uiNumber & 0x000000ff) << 24) | (uiNumber >> 24) | ((uiNumber & 0x00ff0000) >> 8) | ((uiNumber & 0x0000ff00) << 8));
		//}
	}
}
}
