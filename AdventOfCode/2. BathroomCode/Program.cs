using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

//  V.Tarvus
//  3.12.2016
//
//  Link to the problem: http://adventofcode.com/2016/day/2
//
//  Errorhandling is not implemented, 
//  since given input is clean, and
//  this program does not need to handle
//  user input..

namespace CodeSolver
{
	public abstract class Pad
	{
		public string[][] buttons;
		public int X;
		public int Y;
	}

	public class NumPad : Pad
	{
		public NumPad()
		{
			buttons = new[]
                {
                new[] {"7", "8", "9"},
                new[] {"4", "5", "6"},
                new[] {"1", "2", "3"}
                };
			X = 1;
			Y = 1;
		}
	}

	public class BonusPad : Pad
	{
		public BonusPad()
		{
			buttons = new[]
                {
                    new[] {"", "", "D", "", ""},
                    new[] {"", "A", "B", "C", ""},
                    new[] {"5", "6", "7", "8", "9"},
                    new[] {"", "2", "3", "4", ""},
                    new[] {"", "", "1", "", ""},
                };
			X = 0;
			Y = 2;
		}
	}

	/// <summary>
	/// Execute twice, for phase one with basic numpad,
	/// for phase two with "BonusPad".
	/// Read directions and find out the next button to press.
	/// </summary>
	static class Program
	{
		private static Pad pad;
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();

			for (int phase = 1; phase <= 2; phase++)
			{
				switch (phase)
				{
					case 1:
						pad = new NumPad();
						break;
					case 2:
						pad = new BonusPad();
						break;
				}
				string TheCode = String.Empty;

				using (FileStream fs = new FileStream("input.txt", FileMode.Open, FileAccess.Read))
				using (StreamReader sr = new StreamReader(fs))
				{
					while (!sr.EndOfStream)
					{
						string directions = sr.ReadLine();
						TheCode += MoveInPad(directions);
					}
				}
				Console.WriteLine("The bathroom code in phase {0} is {1}", phase, TheCode);
			}

			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalSeconds);
		}

		/// <summary>
		/// Moves within pad to get the next button
		/// for the code
		/// </summary>
		/// <param name="directions">Directions</param>
		/// <returns>String that reads in a button</returns>
		private static string MoveInPad(string directions)
		{
			foreach (char step in directions)
			{
				try
				{
					// NullOrEmpty handles situations where directions
					// lead to empty strings meaning "out of the pad"
					// and try-catch handles the cases when the code
					// actually goes "out of the pad"
					switch (step)
					{
						case 'U':
							if (!String.IsNullOrEmpty(pad.buttons.ElementAt(pad.Y + 1).ElementAt(pad.X)))
								++pad.Y;
							break;
						case 'D':
							if (!String.IsNullOrEmpty(pad.buttons.ElementAt(pad.Y - 1).ElementAt(pad.X)))
								--pad.Y;
							break;
						case 'R':
							if (!String.IsNullOrEmpty(pad.buttons.ElementAt(pad.Y).ElementAt(pad.X + 1)))
								++pad.X;
							break;
						case 'L':
							if (!String.IsNullOrEmpty(pad.buttons.ElementAt(pad.Y).ElementAt(pad.X - 1)))
								--pad.X;
							break;
						default: // Not possible in this input data... 
							throw new Exception("Bad input");
					}
				}
				catch (Exception) { } // This just silences the "out of the pads", which should be ignored
			}
			return pad.buttons.ElementAt(pad.Y).ElementAt(pad.X);
		}
	}
}
