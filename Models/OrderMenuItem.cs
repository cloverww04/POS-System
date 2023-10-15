namespace wangazon.Models
{
    public class OrderMenuItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? Comment { get; set; }
        public MenuItem? MenuItem { get; set; }
    }
}
