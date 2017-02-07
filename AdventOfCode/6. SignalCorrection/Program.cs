using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SignalCorrection
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (FileStream fs = new FileStream("input.txt", FileMode.Open, FileAccess.Read))
			using (StreamReader sr = new StreamReader(fs))
			{
				List<Dictionary<char, int>> values = new List<Dictionary<char, int>>();
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					for (int i = 0; i < line.Length; ++i)
					{
						if (values.Count == i)
						{
							values.Add(new Dictionary<char, int>());
						}
						if (values[i].ContainsKey(line[i]))
						{
							values[i][line[i]]++;
						}
						else
						{
							values[i].Add(line[i], 1);
						}
					}
				}
				char[] message = new char[values.Count];
				char[] message2 = new char[values.Count];
				for (int i = 0; i < values.Count; i++)
				{
					message[i] = values[i].ToList().OrderByDescending(x => x.Value).Select(y => y.Key).Take(1).ToArray()[0];
					message2[i] = values[i].ToList().OrderBy(x => x.Value).Select(y => y.Key).Take(1).ToArray()[0];
				}
				Console.WriteLine("Phase 1: " + new string(message));
				Console.WriteLine("Phase 2: " + new string(message2));
			}
			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalSeconds);
		}
	}
}
