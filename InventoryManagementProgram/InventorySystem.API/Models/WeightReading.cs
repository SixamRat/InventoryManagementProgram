namespace InventorySystem.API.Models
{
    public class WeightReading
    {
        public int Id { get; set; }
        public double WeightInKg { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int ScaleId { get; set; }
        public Scale Scale { get; set; } = null!;
    }
}
