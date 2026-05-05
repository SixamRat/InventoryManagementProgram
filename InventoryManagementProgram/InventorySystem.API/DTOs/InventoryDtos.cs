namespace InventorySystem.API.DTOs
{
     public class InventoryItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double TotalQuantity { get; set; }
        public int NumberOfScales { get; set; }
    }

    public class InventoryListDto
    {
        public string? TeamName { get; set; }
        public DateTime GeneratedAt { get; set; }
        public List<InventoryItemDto> Items { get; set; } = new();
    }
}