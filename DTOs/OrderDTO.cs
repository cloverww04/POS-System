namespace wangazon.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string? OrderName { get; set; }
        public string? OrderStatus { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? OrderType { get; set; }
        public string? Comment { get; set; }
        public List<MenuItemDTO>? MenuItems { get; set; }
        public decimal TotalOrderAmount { get; set; }
    }
}
