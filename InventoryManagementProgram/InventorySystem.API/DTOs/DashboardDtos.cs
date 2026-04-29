namespace InventorySystem.API.DTOs
{
    public class DashboardScaleDto
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string? ProductName { get; set; }
        public string? ProductUnit { get; set; }
        public double? LatestWeightKg { get; set; }
        public double? ConvertedValue { get; set; }
        public DateTime? LastReading { get; set; }
    }
}