namespace AlmostengrLightShow
{
    public class Processor
    {
        public string StanitizeCsvValue(string inputString)
        {
            inputString = inputString.Replace("\"", "");
            return inputString;
        }
    }
}