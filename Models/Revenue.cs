namespace wangazon.Models
{
    public class Revenue
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<Order>? Orders { get; set; }


        public decimal TotalTips
        {
            get
            {
                if (Orders != null)
                {
                    decimal totalTips = 0.00m;

                    foreach (var order in Orders)
                    {
                        if (order.Tip > 0)
                        {
                            totalTips += order.Tip;
                        }
                    }

                    return totalTips;
                }

                return 0.00m;
            }
        }
    }
}
