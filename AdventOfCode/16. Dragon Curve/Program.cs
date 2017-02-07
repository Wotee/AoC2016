using System;
using System.Collections;
using System.Diagnostics;

namespace _16.Dragon_Curve
{
	class Program
	{
		static void Main(string[] args)
		{
			bool test = false;
			Stopwatch watch = new Stopwatch();
			watch.Start();
			int phase1Length = 272;
			int phase2Length = 35651584;
			string data = args.Length != 0 ? args[0] : "11110010111001001"; // Puzzle input via paramter or hardcoded my default value
			bool[] array = new bool[data.Length];

			for (int i = 0; i < array.Length; i++)
			{
				array[i] = data[i] == '1';
			}

			while (array.Length <= phase1Length)
			{
				array = DragonCurve(array);
			}
			CalculateChecksum(array, phase1Length);

			while (array.Length <= phase2Length)
			{
				array = DragonCurve(array);
			}
			CalculateChecksum(array, phase2Length);

			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalSeconds);

		}

		static bool[] DragonCurve(bool[] array)
		{
			bool[] a = array;
			bool[] b = new bool[a.Length];
			Array.Copy(a, b, a.Length);
			Array.Reverse(b);
			BitArray BitB = new BitArray(b);
			BitB.Not().CopyTo(b, 0);
			array = new bool[2 * a.Length + 1];
			Array.Copy(a, array, a.Length);
			array[a.Length] = false;
			Array.Copy(b, 0, array, a.Length + 1, a.Length);
			return array;
		}

		static void CalculateChecksum(bool[] array, int length)
		{
			char[] checkSumData = new char[length];
			Array.Copy(Array.ConvertAll(array, x => x ? '1' : '0'), 0, checkSumData, 0, length);
			char[] checkSum;
			do
			{
				checkSum = new char[checkSumData.Length / 2];
				for (int i = 0, j = 0; i < checkSumData.Length; i += 2, j++)
				{
					checkSum[j] = checkSumData[i] == checkSumData[i + 1] ? '1' : '0';
				}
				checkSumData = new char[checkSum.Length];
				Array.Copy(checkSum, checkSumData, checkSumData.Length);
			} while (checkSum.Length % 2 == 0);
			Console.WriteLine(new string(checkSumData));
		}
	}
}
