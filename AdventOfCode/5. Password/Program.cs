using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Password
{
    public abstract class PasswordParser
    {
        public Mutex mutex;
        public string[] password;
        public bool[] status;

        private Thread WriterThread;

        public PasswordParser()
        {
            password = new string[8];
            status = new [] { false, false, false, false, false, false, false, false };
            mutex = new Mutex(false, "Mutex");
            WriterThread = new Thread(new ParameterizedThreadStart(ProgressWriter.Write));
            WriterThread.Start(this);
        }

        ~PasswordParser()
        {
            WriterThread.Abort();
            mutex.Dispose();
        }

        public abstract void generatePassword(string input);

        protected string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }

    public class SimpleParser : PasswordParser
    {
        public override void generatePassword(string input)
        {
            using (MD5 md5hash = MD5.Create())
            {
                int location = 0;
                for (int i = 0; i < int.MaxValue; i++)
                {
                    string hash = GetMd5Hash(md5hash, input + i);
                    if (hash.StartsWith("00000"))
                    {
                        mutex.WaitOne();
                        password[location] = (hash[5].ToString());
                        status[location] = true;
                        mutex.ReleaseMutex();
                        ++location;
                        if (location == 8)
                            break;
                    }
                }
            }
        }
    }

    public class LocationParser : PasswordParser
    {   
        public override void generatePassword(string input)
        {
            using (MD5 md5hash = MD5.Create())
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    string hash = GetMd5Hash(md5hash, input + i);
                    if (hash.StartsWith("00000"))
                    {
                        int location;
                        mutex.WaitOne();
                        if (IsValidLocation(hash[5], out location) && String.IsNullOrEmpty(password[location]))
                        {
                            password[location] = hash[6].ToString();
                            status[location] = true;
                            bool allDone = true;
                            foreach (bool charStatus in status)
                            {
                                if (charStatus == false)
                                {
                                    allDone = false;
                                    break;
                                }
                            }
                            if (allDone)
                            {
                                mutex.ReleaseMutex();
                                break;
                            }
                        }
                        mutex.ReleaseMutex();
                    }
                }
            }
        }

        private bool IsValidLocation(char p, out int location)
        {
            if (Int32.TryParse(p.ToString(), out location))
            {
                if (location < 8 && location >= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string input = args[0];
            PasswordParser parser;
            for (int phase = 1; phase <= 2; phase++)
            {
                try
                {
                    Console.WriteLine("Phase " + phase + ":");
                    switch (phase)
                    {
                        case 1:
                            parser = new SimpleParser();
                            break;
                        case 2:
                            parser = new LocationParser();
                            break;
                        default:
                            throw new Exception("Unknown parser");
                    }
                    parser.generatePassword(input);
                    Thread.Sleep(350);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unknown parser.. Aborting..");
                }
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalSeconds);
        }
    }

    public static class ProgressWriter
    {
        private static bool AllDone(bool[] status)
        {
            foreach (bool charStatus in status)
            {
                if (charStatus == false)
                    return false;
            }
            return true;
        }

        public static void Write(object parser)
        {
            PasswordParser passwordParser = (PasswordParser)parser;
            int counter = 0;
            Console.WriteLine("Decyphering...");
            bool WriterDone = false;
            while (!WriterDone)
            {
                Thread.Sleep(250);
                string rollAnimation = "";

                switch (counter % 4)
                {
                    case 0:
                        rollAnimation = "/";
                        break;
                    case 1:
                        rollAnimation = "-";
                        break;
                    case 2:
                        rollAnimation = "\\";
                        break;
                    case 3:
                        rollAnimation = "|";
                        counter = -1;
                        break;
                }
                counter++;
                passwordParser.mutex.WaitOne();
                Console.Write("\rPassword: {0}{1}{2}{3}{4}{5}{6}{7}",
                    String.IsNullOrEmpty(passwordParser.password[0]) ? rollAnimation : passwordParser.password[0],
                    String.IsNullOrEmpty(passwordParser.password[1]) ? rollAnimation : passwordParser.password[1],
                    String.IsNullOrEmpty(passwordParser.password[2]) ? rollAnimation : passwordParser.password[2],
                    String.IsNullOrEmpty(passwordParser.password[3]) ? rollAnimation : passwordParser.password[3],
                    String.IsNullOrEmpty(passwordParser.password[4]) ? rollAnimation : passwordParser.password[4],
                    String.IsNullOrEmpty(passwordParser.password[5]) ? rollAnimation : passwordParser.password[5],
                    String.IsNullOrEmpty(passwordParser.password[6]) ? rollAnimation : passwordParser.password[6],
                    String.IsNullOrEmpty(passwordParser.password[7]) ? rollAnimation : passwordParser.password[7]);
                WriterDone = AllDone(passwordParser.status);
                passwordParser.mutex.ReleaseMutex();
            }
            Console.WriteLine();
        }
    }
}
