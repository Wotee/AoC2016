using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace _8.Screen
{
	public class Screen
	{
		public char[][] pixels;
		public int litPixelCount;
		public int CursorLeft;
		public int CursorTop;
		public Screen()
		{
			CursorLeft = Console.CursorLeft;
			CursorTop = Console.CursorTop;
			Console.CursorVisible = false;
			litPixelCount = 0;
			pixels = new char[6][];
			for (int i = 0; i < pixels.Length; ++i)
			{
				pixels[i] = new[]
                {
                    '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
                    '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
                    '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
                    '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
                    '.', '.', '.', '.', '.', '.', '.', '.', '.', '.'
                };
			}
			UpdateScreen();
		}

		~Screen()
		{
			Console.CursorVisible = true;
		}

		public void CreatePixels(string size)
		{
			int x = Int32.Parse(size.Substring(0, size.IndexOf('x')));
			int y = Int32.Parse(size.Substring(size.IndexOf('x') + 1));
			if (x > 1 && y == 1)
			{
				for (int i = 0; i < x; i++)
				{
					pixels[0][i] = '#';
					UpdateScreen();
				}
			}
			else if (y > 1 && x == 1)
			{
				for (int i = 0; i < y; i++)
				{
					pixels[i][0] = '#';
					UpdateScreen();
				}
			}
			else
			{
				for (int i = 0; i < y; i++)
				{
					for (int j = 0; j < x; j++)
					{
						pixels[i][j] = '#';
						++litPixelCount;
					}
				}
				UpdateScreen();
			}
		}

		private void UpdateScreen()
		{
			Thread.Sleep(25);
			Console.CursorLeft = CursorLeft;
			Console.CursorTop = CursorTop;
			string test = "\n";
			foreach (var line in pixels)
			{
				test += new string(line) + "\n";
			}

			using (var stream = new StreamWriter(Console.OpenStandardOutput()))
			{
				stream.Write(test);
			}
		}

		internal void MoveColumnPixels(string line)
		{
			int column = Int32.Parse(line.Substring(0, line.IndexOf(' ')));
			int amount = Int32.Parse(line.Substring(line.LastIndexOf(' ')));
			char prev = ' ';
			for (int i = 0; i < amount; i++)
			{
				for (int j = 0; j < pixels.Length; j++)
				{
					if (j == 0)
					{
						prev = pixels[j][column];
						continue;
					}
					pixels[j][column] ^= prev;
					prev ^= pixels[j][column];
					pixels[j][column] ^= prev;
				}
				pixels[0][column] = prev;
				UpdateScreen();
			}
		}

		internal void MoveRowPixels(string line)
		{
			int row = Int32.Parse(line.Substring(0, line.IndexOf(' ')));
			int amount = Int32.Parse(line.Substring(line.LastIndexOf(' ')));
			char prev = ' ';
			for (int i = 0; i < amount; i++)
			{
				for (int j = 0; j < pixels[row].Length; j++)
				{
					if (j == 0)
					{
						prev = pixels[row][j];
						continue;
					}
					pixels[row][j] ^= prev;
					prev ^= pixels[row][j];
					pixels[row][j] ^= prev;
				}
				pixels[row][0] = prev;
				UpdateScreen();
			}
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (FileStream fs = new FileStream("input.txt", FileMode.Open, FileAccess.Read))
			using (StreamReader sr = new StreamReader(fs))
			{
				Screen screen = new Screen();
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					if (line.StartsWith("rect"))
						screen.CreatePixels(line.Substring(5));
					else if (line.StartsWith("rotate column"))
						screen.MoveColumnPixels(line.Substring(line.IndexOf('=') + 1));
					else if (line.StartsWith("rotate row"))
						screen.MoveRowPixels(line.Substring(line.IndexOf('=') + 1));
				}
			}
			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalSeconds);
		}
	}
}
