namespace wangazon.DTOs
{
    public class RevenueDTO
    {
        public decimal Tip { get; set; }
        public decimal TotalOrderAmountWithTip { get; set; }
        public string? PaymentType { get; set; }
        public string? OrderType { get; set; }
        public DateTime OrderClosed { get; set; }
    }
}
