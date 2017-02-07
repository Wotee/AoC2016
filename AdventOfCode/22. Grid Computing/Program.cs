using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _22.Grid_Computing
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] input = File.ReadAllLines("input.txt");
			int valid = 0;
			Regex regex = new Regex(@"\S+\s+(\d+)T\s+(\d+)T\s+(\d)");
			for (int i = 2; i < input.Length; i++) // First two lines are not input..
			{
				var match = regex.Match(input[i]);
				int size = int.Parse(match.Groups[2].Value);
				if (size == 0) continue; // A can't be zero
				for (int j = i + 1; j < input.Length; j++) // A can't be B
				{
					var match2 = regex.Match(input[j]);
					if (size < int.Parse(match2.Groups[3].Value))
						valid++;
				}
			}
			Console.WriteLine(valid);
		}
	}
}
