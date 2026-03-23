namespace InventorySystem.API.DTOs
{
    public class CreateScaleDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public int TeamId { get; set; }
    }
}
