namespace wangazon.Models
{
    public class Revenue
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<Order>? Orders { get; set; }



        public Dictionary<string, int> OrderTypeCounts
        {
            get
            {
                var typeCounts = new Dictionary<string, int>();

                if (Orders != null)
                {
                    foreach (var order in Orders)
                    {
                        if (order.Type != null)
                        {
                            foreach (var orderTypes in order.Type)
                            {
                                string types = orderTypes.Type ?? "Unknown";

                                if (typeCounts.ContainsKey(types))
                                {
                                    typeCounts[types]++;
                                }
                                else
                                {
                                    typeCounts[types] = 1;
                                }
                            }
                        }
                    }
                }

                return typeCounts;
            }
        }

        public Dictionary<string, int> PaymentTypeCounts
        {
            get
            {
                var paymentTypeCounts = new Dictionary<string, int>();

                if (Orders != null)
                {
                    foreach (var order in Orders)
                    {
                        if (order.PaymentTypes != null)
                        {
                            foreach (var paymentType in order.PaymentTypes)
                            {
                                string type = paymentType.Type ?? "Unknown";

                                if (paymentTypeCounts.ContainsKey(type))
                                {
                                    paymentTypeCounts[type]++;
                                }
                                else
                                {
                                    paymentTypeCounts[type] = 1;
                                }
                            }
                        }
                    }
                }

                return paymentTypeCounts;
            }
        }

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
