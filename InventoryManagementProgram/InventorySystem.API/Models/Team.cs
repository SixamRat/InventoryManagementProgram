namespace InventorySystem.API.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Scale> Scales { get; set; } = new List<Scale>();
    }
}
