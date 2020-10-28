namespace Almostengr.Christmaslightshow.ShowConductor
{
    class Program
    {
        static void Main(string[] args)
        {
            Conductor conductor = new Conductor();
            while (true)
            {
                conductor.CheckClock();
            }
        }
    }
}
