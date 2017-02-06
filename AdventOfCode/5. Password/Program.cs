using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Password
{
	static class Program
	{
		private static readonly uint[] T = new uint[64]
		{
			0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
			0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
			0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
			0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
			0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
			0xd62f105d, 0x2441453, 0xd8a1e681, 0xe7d3fbc8,
			0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
			0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
			0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
			0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
			0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x4881d05,
			0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
			0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
			0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
			0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
			0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
		};

		private static int phase1Counter = 0;
		private static int phase2Counter = 0;
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			string input = args.Length != 0 ? args[0] : "uqwqemis"; // Puzzle input via paramter or hardcoded my value
			bool phase2Done = false;
			byte[] pass1 = new byte[8];
			char[] pass2 = {'-','-','-','-','-','-','-','-'};
			
			Parallel.ForEach(Enumerable.Range(1,Int32.MaxValue), 
				(j, parallelLoopState) =>
				{
					byte[] firstFourBytes = GetStartOfFingerprint(input + j);
					if (firstFourBytes[0] + firstFourBytes[1] + (firstFourBytes[2] >> 4) == 0)
					{
						if (phase1Counter < 8)
						{
							pass1[phase1Counter] = firstFourBytes[2];
							phase1Counter++;
						}
						int location = firstFourBytes[2];
						if (location < 8 && location >= 0 && pass2[location] == '-')
						{
							// First half
							byte b = ((byte) (firstFourBytes[3] >> 4));
							pass2[location] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30); ;
							phase2Counter++;
							if (phase2Counter == 8) phase2Done = true;
						}
					}
					if(phase2Done) parallelLoopState.Stop();
				});

			Console.WriteLine(pass1.ToHex());
			Console.WriteLine(new string(pass2));
			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalMilliseconds);
		}

		public static string ToHex(this byte[] bytes)
		{
			char[] c = new char[bytes.Length];
			for (int bx = 0; bx < bytes.Length; ++bx)
			{
				int b = bytes[bx] & 0x0F;
				c[bx] = (char)(b > 0x09 ? b + 0x37 + 0x20 : b + 0x30);
			}
			return new string(c);
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
				byteInput[i] = (byte) input[i];

			byte[] bMsg; //buffer to hold bits

			// create a buffer with bits padded and length is also padded
			uint pad; //no of padding bits for 448 mod 512 
			ulong sizeMsg; //64 bit size pad
			uint sizeMsgBuff; //buffer size in multiple of bytes
			int temp = (448 - ((byteInput.Length * 8) % 512)); //temporary 

			// Get the size of pad
			pad = (uint) ((temp + 512) % 512);
			if (pad == 0)
				pad = 512;

			sizeMsgBuff = (uint) ((byteInput.Length) + (pad / 8) + 8);
			sizeMsg = (ulong) byteInput.Length * 8;
			bMsg = new byte[sizeMsgBuff];

			for (int i = 0; i < byteInput.Length; i++)
				bMsg[i] = byteInput[i];

			// making first bit of padding 1,
			bMsg[byteInput.Length] |= 0x80;

			//writing the size value
			for (int i = 8; i > 0; i--)
				bMsg[sizeMsgBuff - i] = (byte) (sizeMsg >> ((8 - i) * 8) & 0x00000000000000ff);

			for (uint j = 0; j < 61; j += 4)
				X[j >> 2] = (uint) (((bMsg[j + 3]) << 24) | ((bMsg[j + 2]) << 16) | ((bMsg[j + 1]) << 8) | ((bMsg[j])));

			uint AA = A;
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

			A += AA;
			return BitConverter.GetBytes(A);
		}
	}
}
