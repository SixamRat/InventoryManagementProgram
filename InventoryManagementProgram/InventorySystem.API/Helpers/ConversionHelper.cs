namespace InventorySystem.API.Helpers
{    
    // Hjälpklass för att konvertera vikter till rätt enhet.
    public static class ConversionHelper
    {       
        // Räknar om vikt i kg till angiven enhet med ConversionFactor.
        public static double Convert(double weightInKg, double conversionFactor)
        {
            if (weightInKg < 0)
                return 0;

            return Math.Round(weightInKg * conversionFactor, 2);
        }        
        // Summerar flera vikter och avrundar till 2 decimaler.
      
        public static double SumConverted(IEnumerable<double> convertedValues)
        {
            return Math.Round(convertedValues.Sum(), 2);
        }
    }
}