using System;

namespace Almostengr.Christmaslightshow
{
    class Program
    {
        static void Main(string[] args)
        {
            // if (args.Length > 0)
            // {
            //     MainArgs(args);
            // }
            // else
            // {
            //     bool showMenu = true;
            //     while (showMenu)
            //     {
            //         MainMenu();
            //     }
            // }

            LightShow lightShow = new LightShow();
            lightShow.IsItShowtime();




        }

        private static void MainArgs(string[] args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "--runschedule":
                        break;
                    case "--runforce":
                        break;
                    default:
                        break;
                }
            }
        }

        private static bool MainMenu()
        {
            Console.WriteLine(DateTime.Now + ": ----- MAIN MENU -----");
            Console.WriteLine(DateTime.Now + ": 1) Run Show per Schedule");
            Console.WriteLine(DateTime.Now + ": 2) Run Show by Force");
            Console.WriteLine(DateTime.Now + ": 0) Exit");
            Console.WriteLine();
            Console.Write("Enter your selection: ");

            switch (Console.ReadLine().Trim())
            {
                case "1":
                    break;
                case "2":
                    break;
                case "0":
                    return false;
                default:
                    Console.WriteLine(DateTime.Now + ": Invalid selection. Try again");
                    break;
            }

            return true;
        }
    }
}
