using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

//  V.Tarvus
//  3.12.2016
//
//  Link to the problem: http://adventofcode.com/2016/day/1
//
//  Errorhandling is not implemented, 
//  since given input is clean, and
//  this program does not need to handle
//  user input..

namespace Map
{
	public enum CardinalPoints
	{
		North,
		East,
		South,
		West,
	};

	class Program
	{
		static CardinalPoints m_facingDirection = CardinalPoints.North;
		static int m_Xcoordinate = 0;
		static int m_Ycoordinate = 0;
		static bool m_FirstSecondVisit = false;
		static List<string> m_visitedPoints = new List<string>();

		/// <summary>
		/// Read instructions one at a time,
		/// Turn to wanted direction and advance corresponding steps
		/// while marking locations to track visited spots.
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (StreamReader sr = new StreamReader("input.txt"))
			{
				string instructions = String.Empty;
				while (sr.Peek() >= 0)
				{
					char c = (char)sr.Read();
					if (c == ' ') continue;// Skip spaces
					else if (c == ',')
					{
						string direction = instructions.Substring(0, 1);
						int length = Int32.Parse(instructions.Substring(1));
						turn(direction);
						advance(length);
						instructions = String.Empty;
					}
					else
					{
						instructions += c;
					}
				}
				Console.WriteLine("Final distance is: " + distance());
				watch.Stop();
				Console.WriteLine(watch.Elapsed.TotalSeconds);
			}
		}

		/// <summary>
		/// Gets the distance from coordinate 0,0
		/// </summary>
		/// <returns>Distance from coordinate</returns>
		private static string distance()
		{
			return (Math.Abs(m_Xcoordinate) + Math.Abs(m_Ycoordinate)).ToString();
		}

		/// <summary>
		/// Advances in coordinates.
		/// Writes coordinates down to find out,
		/// which coordinate is the first one to be visited twice
		/// </summary>
		/// <param name="length">Steps to advance</param>
		private static void advance(int length)
		{
			for (int i = 0; i < length; i++)
			{
				switch (m_facingDirection)
				{
					case CardinalPoints.North:
						m_Xcoordinate++;
						break;
					case CardinalPoints.East:
						m_Ycoordinate++;
						break;
					case CardinalPoints.South:
						m_Xcoordinate--;
						break;
					case CardinalPoints.West:
						m_Ycoordinate--;
						break;
				}
				if (!m_FirstSecondVisit)
				{
					string visitedPoint = m_Xcoordinate + "," + m_Ycoordinate;
					if (m_visitedPoints.Contains(visitedPoint))
					{
						m_FirstSecondVisit = true;
						Console.WriteLine("Distance to first spot visited twice: " + distance());
					}
					m_visitedPoints.Add(visitedPoint);
				}
			}
		}

		/// <summary>
		/// Turns m_facingDirection to left or rigth
		/// </summary>
		/// <param name="direction">"L" or "R"</param>
		public static void turn(string direction)
		{
			if (direction == "R")
				++m_facingDirection;
			else if (direction == "L")
				--m_facingDirection;
			if ((int)m_facingDirection == -1)
				m_facingDirection = CardinalPoints.West;
			else if ((int)m_facingDirection == 4)
				m_facingDirection = CardinalPoints.North;
		}
	}
}
