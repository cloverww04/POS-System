namespace wangazon.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Uid { get; set; }

        public ICollection<Revenue>? Revenues { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
