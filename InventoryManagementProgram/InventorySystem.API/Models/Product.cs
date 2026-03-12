namespace InventorySystem.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty; // Liter, Kg, Styck
        public double ConversionFactor { get; set; } // Konverterar kg till vald enhet

        public int ScaleId { get; set; }
        public Scale Scale { get; set; } = null!;
    }
}