namespace InventorySystem.API.Models
{
    public class Scale
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public Product? Product { get; set; }
        public ICollection<WeightReading> WeightReadings { get; set; } = new List<WeightReading>();
        public ICollection<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
    }
}
