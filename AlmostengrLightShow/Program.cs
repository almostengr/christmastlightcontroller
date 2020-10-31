namespace AlmostengrLightShow
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger logger = new ConsoleLogger();
            LightingShow lightingShow = new LightingShow(logger);

            if (args.Length == 0)
            {
                logger.ShowHelp();
            }
            else
            {
                lightingShow.RunShow(args);
            }
        }
    }
}
