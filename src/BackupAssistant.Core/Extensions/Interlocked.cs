namespace BackupAssistant.Extensions
{
    public static class Interlocked
    {
        public static float Add(ref float location1, float value)
        {
            float newCurrentValue;
            float currentValue;

            do
            {
                currentValue = location1;
                newCurrentValue = currentValue + value;
            }
            while (System.Threading.Interlocked.CompareExchange(ref location1, newCurrentValue, currentValue) != currentValue);

            return newCurrentValue;
        }
    }
}
