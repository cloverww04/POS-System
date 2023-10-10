namespace wangazon.DTOs
{
    public class CreateOrderDTO
    {
        public int EmployeeId { get; set; }
        public DateTime? OrderPlaced { get; set; }
        public DateTime? OrderClosed { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public decimal Tip { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Review { get; set; }
    }
}
