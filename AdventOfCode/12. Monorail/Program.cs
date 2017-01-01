using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Monorail
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex digit = new Regex(@"\D");
            string[] input = File.ReadAllLines("input.txt");
            for (int phase = 1; phase <= 2; phase++)
            {
                int[] registers = (phase == 1) ? new int[4] : new[] {0, 0, 1, 0};
                for (int i = 0; i < input.Length; i++)
                {
                    string[] instructions = input[i].Split(' ');
                    switch (instructions[0])
                    {
                        case "cpy":
                            if (digit.IsMatch(instructions[1]))
                                registers[instructions[2][0] - 97] = registers[instructions[1][0] - 97];
                            else
                                registers[instructions[2][0] - 97] = Convert.ToInt32(instructions[1]);
                            break;
                        case "inc":
                            registers[instructions[1][0] - 97]++;
                            break;
                        case "dec":
                            registers[instructions[1][0] - 97]--;
                            break;
                        case "jnz":
                            if (digit.IsMatch(instructions[1]))
                            {
                                if (registers[instructions[1][0] - 97] != 0)
                                    i += Convert.ToInt32(instructions[2]) - 1;
                            }
                            else
                            {
                                if (instructions[1] != "0")
                                    i += Convert.ToInt32(instructions[2]) - 1;
                            }
                            break;
                    }
                }
                Console.WriteLine("Phase " + phase + ": " +registers[0]);
            }
            Console.Read();
        }
    }
}
