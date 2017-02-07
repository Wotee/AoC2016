using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace _20.IPAddresses
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			var IPs = File.ReadAllLines("input.txt")
					.OrderBy(x => UInt32.Parse(x.Split('-')[0]))
					.ThenBy(y => UInt32.Parse(y.Split('-')[1]))
					.Select(z => new KeyValuePair<uint, uint>(UInt32.Parse(z.Split('-')[0]), UInt32.Parse(z.Split('-')[1])))
					.ToList();

			uint low = 0;
			bool answer = false;
			uint amount = 0;
			while (IPs.Count != 0) // This can be converted to more elegant for-loop I belive.
			{
				int i;
				for (i = 0; i < IPs.Count; i++)
				{
					if (low < IPs[i].Key) break;
					if (low <= IPs[i].Value)
						low = IPs[i].Value == UInt32.MaxValue ? IPs[i].Value : IPs[i].Value + 1;
				}
				if (!answer)
				{
					Console.WriteLine("Phase 1: Lowest valid IP is " + low);
					answer = true;
				}
				IPs.RemoveRange(0, i);
				if (IPs.Count != 0)
				{
					amount += IPs[0].Key - low;
					low = IPs[0].Value == UInt32.MaxValue ? IPs[0].Value : IPs[0].Value + 1;
				}

			}
			Console.WriteLine("Phase 2: Amount of valid IPs is " + amount);
			watch.Stop();
			Console.WriteLine(watch.Elapsed.TotalSeconds);
			Console.Read();
		}
	}
}