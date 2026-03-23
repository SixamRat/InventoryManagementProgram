namespace InventorySystem.API.DTOs
{
    public class UpdateScaleDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public int TeamId { get; set; }
    }
}