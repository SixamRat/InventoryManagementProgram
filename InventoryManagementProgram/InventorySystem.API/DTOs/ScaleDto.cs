using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace InventorySystem.API.DTOs
{
    public class ScaleDto
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string? ProductName {  get; set; }
        public string? ProductUnit { get; set; }
    }
}
