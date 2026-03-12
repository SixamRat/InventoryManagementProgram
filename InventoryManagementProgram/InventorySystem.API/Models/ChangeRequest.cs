namespace InventorySystem.API.Models
{
    public class ChangeRequest
    {
        public int Id { get; set; }
        public string ProposedProduct { get; set; } = string.Empty;
        public string ProposedUnit { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

        public int ScaleId { get; set; }
        public Scale Scale { get; set; } = null!;

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;
    }
}