using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace _21.Scramble
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();

			string[] input = File.ReadAllLines("input.txt");
			Thread phase1 = new Thread(() => CalculatePhase1(ref input));
			Thread phase2 = new Thread(() => CalculatePhase2(ref input));
			phase1.Start();
			phase2.Start();
			phase1.Join();
			phase2.Join();
			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalMilliseconds);
		}

		private static void CalculatePhase2(ref string[] input)
		{
			var scramble = "fbgdceah".ToCharArray();
			foreach (string line in input.Reverse())
			{
				Regex instruction = new Regex(@"\w+");
				string left;
				string right;
				Regex values;
				if (line.StartsWith("swap", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"swap \w+ (.) with \w+ (.)");
					left = values.Match(line).Groups[1].Value;
					right = values.Match(line).Groups[2].Value;
					scramble.Swap(right, left);
				}
				else if (line.StartsWith("rotate based", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"rotate based on position of letter (\w)");
					left = values.Match(line).Groups[1].Value;
					scramble.RotateBasedPhase2(left[0]);
				}
				else if (line.StartsWith("reverse", StringComparison.OrdinalIgnoreCase)) // TOIMII
				{
					values = new Regex(@"reverse positions (\d+) through (\d+)");
					left = values.Match(line).Groups[1].Value;
					right = values.Match(line).Groups[2].Value;
					scramble.Reverse(Int32.Parse(left), Int32.Parse(right));
				}
				else if (line.StartsWith("rotate", StringComparison.OrdinalIgnoreCase)) // TOIMII
				{
					values = new Regex(@"rotate (\w+) (\d+) step");
					left = values.Match(line).Groups[1].Value == "left" ? "right" : "left";
					right = values.Match(line).Groups[2].Value;
					scramble.Rotate(left, Int32.Parse(right));
				}
				else if (line.StartsWith("move", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"move position (\d+) to position (\d)");
					left = values.Match(line).Groups[1].Value;
					right = values.Match(line).Groups[2].Value;
					scramble.Move(Int32.Parse(right), Int32.Parse(left));
				}
			}
			Console.WriteLine("Phase 2: " + new string(scramble));
		}

		private static void CalculatePhase1(ref string[] input)
		{
			var scramble = "abcdefgh".ToCharArray();
			foreach (string line in input)
			{
				Regex instruction = new Regex(@"\w+");
				string left;
				string right;
				Regex values;
				if (line.StartsWith("swap", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"swap \w+ (.) with \w+ (.)");
					left = values.Match(line).Groups[1].Value;
					right = values.Match(line).Groups[2].Value;
					scramble.Swap(left, right);
				}
				else if (line.StartsWith("rotate based", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"rotate based on position of letter (\w)");
					left = values.Match(line).Groups[1].Value;
					scramble.RotateBased(left[0]);
				}
				else if (line.StartsWith("reverse", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"reverse positions (\d+) through (\d+)");
					left = values.Match(line).Groups[1].Value;
					right = values.Match(line).Groups[2].Value;
					scramble.Reverse(Int32.Parse(left), Int32.Parse(right));
				}
				else if (line.StartsWith("rotate", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"rotate (\w+) (\d+) step");
					left = values.Match(line).Groups[1].Value;
					right = values.Match(line).Groups[2].Value;
					scramble.Rotate(left, Int32.Parse(right));
				}
				else if (line.StartsWith("move", StringComparison.OrdinalIgnoreCase))
				{
					values = new Regex(@"move position (\d+) to position (\d)");
					left = values.Match(line).Groups[1].Value;
					right = values.Match(line).Groups[2].Value;
					scramble.Move(Int32.Parse(left), Int32.Parse(right));
				}
			}
			Console.WriteLine("Phase 1: " + new string(scramble));
		}
	}

	static class ExtensionMethods
	{
		public static void Swap(this char[] charArray, string from, string to)
		{
			int intFrom, intTo;
			if (Int32.TryParse(from, out intFrom) && Int32.TryParse(to, out intTo))
			{
				charArray[intFrom] ^= charArray[intTo];
				charArray[intTo] ^= charArray[intFrom];
				charArray[intFrom] ^= charArray[intTo];
			}
			else
			{
				for (int i = 0; i < charArray.Length; i++)
				{
					if (charArray[i] == from[0])
						charArray[i] = to[0];
					else if (charArray[i] == to[0])
						charArray[i] = from[0];
				}
			}
			return;
		}

		public static void Reverse(this char[] charArray, int from, int to)
		{
			for (int start = from, end = to; start < end; start++, end--)
			{
				charArray[start] ^= charArray[end];
				charArray[end] ^= charArray[start];
				charArray[start] ^= charArray[end];
			}
			return;
		}

		public static void RotateBasedPhase2(this char[] charArray, char character)
		{
			int i = 0;
			while (charArray[i] != character)
				i++;
			switch (i)
			{
				case 0:
				case 1:
					charArray.Rotate("right", 7);
					return;
				case 2:
					charArray.Rotate("right", 2);
					return;
				case 3:
					charArray.Rotate("right", 6);
					return;
				case 4:
					charArray.Rotate("right", 1);
					return;
				case 5:
					charArray.Rotate("right", 5);
					return;
				case 6:
					return;
				case 7:
					charArray.Rotate("right", 4);
					return;
			}
		}

		public static void RotateBased(this char[] charArray, char character)
		{
			int i = 0;
			while (charArray[i] != character)
				i++;
			i += i >= 4 ? 2 : 1;
			charArray.Rotate("right", i);
			return;
		}

		public static void Rotate<T>(this T[] source, string direction, int steps)
		{
			for (int stepsTaken = 0; stepsTaken < steps; stepsTaken++)
			{
				if (direction == "left")
				{
					var temp = source[0];

					for (int i = 0; i < source.Length - 1; i++)
						source[i] = source[i + 1];
					source[source.Length - 1] = temp;
				}
				else
				{
					var temp = source[source.Length - 1];

					for (int i = source.Length - 1; i > 0; i--)
						source[i] = source[i - 1];
					source[0] = temp;
				}
			}
		}

		public static void Move(this char[] charArray, int from, int to)
		{
			if (from < to)
			{
				char temp = charArray[from];
				for (int i = from; i < to; i++)
					charArray[i] = charArray[i + 1];
				charArray[to] = temp;
			}
			else
			{
				char temp = charArray[from];
				for (int i = from; i > to; i--)
					charArray[i] = charArray[i - 1];
				charArray[to] = temp;
			}
		}
	}
}
