namespace wangazon.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime? OrderPlaced { get; set; }
        public DateTime? OrderClosed { get; set; }
        public decimal Tip { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? Review { get; set; }
        public ICollection<OrderType>? Type { get; set; }
        public ICollection<PaymentType>? PaymentTypes { get; set; }
        public ICollection<MenuItem>? MenuItems { get; set; }
        public int? RevenueId { get; set; }
        public Revenue? Revenue { get; set; }

        public Order()
        {
            this.OrderPlaced = DateTime.Now;
            this.OrderClosed = DateTime.Now;
        }

        public decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0.0m;

            if (MenuItems != null)
            {
                foreach (var menuItem in MenuItems)
                {
                    totalPrice += menuItem?.Price ?? 0.0m;
                }
            }

            totalPrice += Tip;

            return totalPrice;
        }




    }
}
