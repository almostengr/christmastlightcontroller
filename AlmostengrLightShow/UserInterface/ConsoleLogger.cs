using static System.DateTime;
using static System.Console;

namespace AlmostengrLightShow
{
    public class ConsoleLogger
    {
        public void Log(string message)
        {
            WriteLine("{0} | {1}", Now, message);
        }

        public void ShowHelp()
        {
            WriteLine("---------- HELP ----------");
            WriteLine();
            WriteLine("Usage:");
            WriteLine("./AlmostengrLightShow -s <effects file name> -p <models file name>");
            WriteLine();
            WriteLine("Additional notes:");
            WriteLine("Effects file needs to be provided.");
            WriteLine("Music file needs to have the same file name, in same folder, and end with '.mp3'.");
        }
    }
}