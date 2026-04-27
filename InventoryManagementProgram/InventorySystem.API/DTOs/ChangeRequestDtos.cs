namespace InventorySystem.API.DTOs
{
   
    public class CreateChangeRequestDto
    {
        public int ScaleId { get; set; }
        public string ProposedProduct { get; set; } = string.Empty;
        public string ProposedUnit { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
    }

    public class ChangeRequestDto
    {
        public int Id { get; set; }
        public string ProposedProduct { get; set; } = string.Empty;
        public string ProposedUnit { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ScaleId { get; set; }
        public string ScaleSerialNumber { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
    }

    
    public class ReviewChangeRequestDto
    {
        public string Status { get; set; } = string.Empty; 
    }
}