namespace wangazon.DTOs
{
    public class RevenueDTO
    {
        public decimal Tip { get; set; }
        public decimal TotalOrderAmountWithTip { get; set; }
        public string? PaymentType { get; set; }
        public string? OrderTypes { get; set; }
        public DateTime? OrderClosed { get; set; }
        public int WalkInCount { get; set; }
        public int CallInCount { get; set; }
        public int CashCount { get; set; }
        public int CreditCardCount { get; set; }

    }
}
