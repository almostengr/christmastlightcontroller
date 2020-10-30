using System.Threading.Tasks;

namespace AlmostengrLightShow
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            ConsoleLogger logger = new ConsoleLogger();
            LightingShow lightingShow = new LightingShow(logger);

            if (args.Length == 0 || args.Length > 3)
            {
                logger.ShowHelp();
                return 1;
            }
            else
            {
                await lightingShow.RunShow(args);
                return 0;
            }
        }
    }
}
