using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Monorail
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex regex = new Regex(@"\D");
            string[] input = File.ReadAllLines("input.txt");
            for (int phase = 1; phase <= 2; phase++)
            {
                int[] registers = (phase == 1) ? new int[4] : new[] {0, 0, 1, 0};
                for (int i = 0; i < input.Length; i++)
                {
                    string[] instructions = input[i].Split(' ');
                    // Do char - 97 to map a -> 0, b -> 1, c -> 2... This makes possible using int[] registers.
                    switch (instructions[0])
                    {
                        case "cpy":
                            if (regex.IsMatch(instructions[1]))
                                registers[Convert.ToChar(instructions[2]) - 97] = registers[Convert.ToChar(instructions[1]) - 97];
                            else
                                registers[Convert.ToChar(instructions[2]) - 97] = Convert.ToInt32(instructions[1]);
                            break;
                        case "inc":
                            registers[Convert.ToChar(instructions[1]) - 97]++;
                            break;
                        case "dec":
                            registers[Convert.ToChar(instructions[1]) - 97]--;
                            break;
                        case "jnz":
                            if (regex.IsMatch(instructions[1]))
                            {
                                if (registers[Convert.ToChar(instructions[1]) - 97] != 0)
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
