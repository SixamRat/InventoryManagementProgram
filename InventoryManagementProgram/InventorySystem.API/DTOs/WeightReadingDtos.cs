namespace InventorySystem.API.DTOs
{
    // "Våg" -> API
    public class CreateWeightReadingDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public double WeightInKg { get; set; }
    }

    // Skickar tillbaka omräknad till rätt enhet
    public class WeightReadingResultDto
    {
        public int ScaleId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public double WeightInKg { get; set; }
        public double ConvertedValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}