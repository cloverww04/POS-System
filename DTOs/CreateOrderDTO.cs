﻿using wangazon.Models;

namespace wangazon.DTOs
{
    public class CreateOrderDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? OrderPlaced { get; set; }
        public DateTime? OrderClosed { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public decimal Tip { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? Review { get; set; }
        public string? Comment { get; set; }
        public List<OrderMenuItem>? OrderMenuItems { get; set; }
    }
}
