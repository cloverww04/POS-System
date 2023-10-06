namespace wangazon.Models
{
    public class PaymentType
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
