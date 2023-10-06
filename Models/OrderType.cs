namespace wangazon.Models
{
    public class OrderType
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
