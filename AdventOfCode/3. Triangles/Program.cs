using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Triangles
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var watch = new Stopwatch();
			watch.Start();
			var input = File.ReadAllLines("input.txt");
			int validTrianglesPhase1 = input.Length, validTrianglesPhase2 = input.Length;
			var regex = new Regex(@"\d+");
			for (var i = 0; i < input.Length; i += 3)
			{
				var matches = regex.Matches(input[i]);
				int[] input1 = {int.Parse(matches[0].Value), int.Parse(matches[1].Value), int.Parse(matches[2].Value)};
				matches = regex.Matches(input[i + 1]);
				int[] input2 = {int.Parse(matches[0].Value), int.Parse(matches[1].Value), int.Parse(matches[2].Value)};
				matches = regex.Matches(input[i + 2]);
				int[] input3 = {int.Parse(matches[0].Value), int.Parse(matches[1].Value), int.Parse(matches[2].Value)};
				// Phase 1
				if (input1[0] + input1[1] <= input1[2] || input1[0] + input1[2] <= input1[1] || input1[1] + input1[2] <= input1[0])
					validTrianglesPhase1++;
				if (input2[0] + input2[1] <= input2[2] || input2[0] + input2[2] <= input2[1] || input2[1] + input2[2] <= input2[0])
					validTrianglesPhase1++;
				if (input3[0] + input3[1] <= input3[2] || input3[0] + input3[2] <= input3[1] || input3[1] + input3[2] <= input3[0])
					validTrianglesPhase1++;
				// Phase 2
				if (input1[0] + input2[0] <= input3[0] || input1[0] + input3[0] <= input2[0] || input2[0] + input3[0] <= input1[0])
					validTrianglesPhase2++;
				if (input1[1] + input2[1] <= input3[1] || input1[1] + input3[1] <= input2[1] || input2[1] + input3[1] <= input1[1])
					validTrianglesPhase2++;
				if (input1[2] + input2[2] <= input3[2] || input1[2] + input3[2] <= input2[2] || input2[2] + input3[2] <= input1[2])
					validTrianglesPhase2++;
			}
			Console.WriteLine("Phase 1: " + validTrianglesPhase1 + " triangles");
			Console.WriteLine("Phase 2: " + validTrianglesPhase2 + " triangles");
			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalMilliseconds);
		}
	}
}