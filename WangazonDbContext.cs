using Microsoft.EntityFrameworkCore;
using wangazon.Models;
namespace wangazon
{
    public class WangazonDbContext : DbContext
    {
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<MenuItem>? MenuItems { get; set; }
        public DbSet<OrderType>? OrderTypes { get; set; }
        public DbSet<PaymentType>? PaymentTypes { get; set; }
        public DbSet<Revenue>? Revenues { get; set; }

        public WangazonDbContext(DbContextOptions<WangazonDbContext> context) : base(context) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Employee>().HasData(new Employee[]
            {
                new Employee { Id = 1, FirstName = "Nathan", LastName = "Clover", Email = "N.clover@email.com"}
            });

            modelBuilder.Entity<OrderType>().HasData(new OrderType[]
            {
                new OrderType { Id = 1, Type = "Call In"},
                new OrderType { Id = 2, Type = "Walk In"}
            });

            modelBuilder.Entity<PaymentType>().HasData(new PaymentType[]
            {
                new PaymentType { Id = 1, Type = "Credit Card"},
                new PaymentType { Id = 2, Type = "Cash"}
            });

            modelBuilder.Entity<MenuItem>().HasData(new MenuItem[]
            {
                new MenuItem { Id = 1, Name = "Pizza", Description = "Pepperoni pizza with extra cheese", Price = 16.99m},
                new MenuItem { Id = 2, Name = "Wangs", Description = "Greasy, fried, and sizzling wangs", Price = 12.99m},
                new MenuItem { Id = 3, Name = "Milk-Shake", Description = "Oreo Milk-Shake with whip cream", Price = 7.99m}
            });

            modelBuilder.Entity<Order>().HasData(new Order[]
            {
                new Order
                {
                    Id = 1,
                    EmployeeId = 1,
                    OrderPlaced = DateTime.Now.AddHours(-2),
                    OrderClosed = DateTime.Now.AddMinutes(-30),
                    Tip = 2.00m,
                    CustomerFirstName = "John",
                    CustomerLastName = "Doe",
                    CustomerPhone = "555-123-4567",
                    CustomerEmail = "john.doe@example.com",
                    Review = "Great service!",

                },

                new Order
                {
                    Id = 2,
                    EmployeeId = 1,
                    OrderPlaced = DateTime.Now.AddHours(-2),
                    OrderClosed = DateTime.Now.AddMinutes(-30),
                    Tip = 5.00m,
                    CustomerFirstName = "Bob",
                    CustomerLastName = "Dole",
                    CustomerPhone = "555-231-1267",
                    CustomerEmail = "bob.dole@example.com",
                    Review = "It was ok",

                }
            });


            modelBuilder.Entity<Order>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeId);

            modelBuilder.Entity<Revenue>()
                .HasMany(r => r.Orders)
                .WithOne(o => o.Revenue)
                .HasForeignKey(o => o.RevenueId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.MenuItems)
                .WithMany(m => m.Orders)
                .UsingEntity(om => om.ToTable("OrderMenuItem"));

            modelBuilder.Entity<OrderType>()
                .HasMany(o => o.Orders)
                .WithMany(ot => ot.Type)
                .UsingEntity(ot => ot.ToTable("OrderOrderTypes"));

            modelBuilder.Entity<PaymentType>()
                .HasMany(o => o.Orders)
                .WithMany(pt => pt.PaymentTypes)
                .UsingEntity(opt => opt.ToTable("OrderPaymentType"));


        }

    }
}
