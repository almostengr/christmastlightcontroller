using System;

namespace AlmostengrLightShow
{
    public class ConsoleLogger
    {
        public void Log(string message)
        {
            Console.WriteLine("{0} | {1}", DateTime.Now, message);
        }

        public void ShowHelp()
        {
            Console.WriteLine("--- HELP ---");
            Console.WriteLine("");
            Console.WriteLine("Usage: ./showconductor -s <data file name>");
            Console.WriteLine("");
            Console.WriteLine("Data file needs to be provided.");
            Console.WriteLine("Music file need to have the same file name, same folder, and end with '.mp3'.");
        }
    }
}