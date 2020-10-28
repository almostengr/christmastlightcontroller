using System;

namespace Almostengr.Christmaslightshow.ShowController.ShowController
{
    class Program
    {
        static void Main(string[] args)
        {
            // if (args.Length == 1)
            if (true)
            {
                string song = "/home/almostengineer/christmaslightshow/xlights/showData/20thCenturyFox.csv";
                // LightController controller = new LightController(args[0]);
                LightController controller = new LightController(song);
                controller.ControlLights();
            }
            else
            {
                Console.WriteLine("Provide a Xlights Sequence export file");
            }
        }
    }
}
