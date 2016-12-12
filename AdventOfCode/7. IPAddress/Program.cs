using System;
using System.IO;
using System.Text.RegularExpressions;

namespace _7.IPAddresses
{
    public interface IProtocolParser
    {
        bool isValidLine(string line);
    }

    public class TLSParser : IProtocolParser
    {
        Regex rgx = new Regex(@".*(.)(?!\1)(.)\2\1");
        Regex rgx2 = new Regex(@".*\[\w*(.)(?!\1)(.)\2\1\w*\]");
        public bool isValidLine(string line)
        {
            if (rgx.IsMatch(line) && !rgx2.IsMatch(line))
                return true;
            return false;
        }
    }

    public class SSLParser : IProtocolParser
    {
        Regex rgx = new Regex(@"(.)(?!\1)(.)\1\w*(([][]\w*){2})*[][]\w*\2\1\2");
        public bool isValidLine(string line)
        {
            if(rgx.IsMatch(line))
                return true;
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IProtocolParser parser1 = new TLSParser();
            IProtocolParser parser2 = new SSLParser();
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                int TLSCounter = 0;
                int SSLCounter = 0;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (parser1.isValidLine(line))
                        ++TLSCounter;
                    if (parser2.isValidLine(line))
                        ++SSLCounter;
                }
                Console.WriteLine("Valid TLS lines: " + TLSCounter);
                Console.WriteLine("Valid SSL lines: " + SSLCounter);
            }
        }
    }
}
