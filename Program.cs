using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using wangazon;
using wangazon.DTOs;
using wangazon.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<WangazonDbContext>(builder.Configuration["WangazonDbConnectionString"]);
// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



// Endpoints for Orders
app.MapGet("/api/orders", async (WangazonDbContext db) =>
{
    var orders = await db.Orders
    .Include(o => o.Type)
    .ToListAsync();

    var ordersDTO = orders.Select(order => new OrderDTO
    {
        OrderName = order.CustomerFirstName + " " + order.CustomerLastName,
        OrderStatus = order.OrderClosed.HasValue ? "Closed" : "Open",
        PhoneNumber = order.CustomerPhone,
        EmailAddress = order.CustomerEmail,
        OrderType = order.Type?.FirstOrDefault()?.Type,

    }).ToList();

    if (orders == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(ordersDTO);
});




app.MapGet("/api/orders/{id}", async (WangazonDbContext db, int id) =>
{
    var order = await db.Orders
        .Include(o => o.Type)
        .Include(o => o.MenuItems)
        .Where(o => o.Id == id)
        .FirstOrDefaultAsync();

    if (order == null)
    {
        return Results.NotFound();
    }

    decimal totalOrderAmount = order.MenuItems.Sum(item => item.Price);

    var orderDTO = new OrderDTO
    {
        OrderName = order.CustomerFirstName + " " + order.CustomerLastName,
        OrderStatus = order.OrderClosed.HasValue ? "Closed" : "Open",
        PhoneNumber = order.CustomerPhone,
        EmailAddress = order.CustomerEmail,
        OrderType = order.Type?.FirstOrDefault()?.Type,
        MenuItems = order.MenuItems.Select(item => new MenuItemDTO
        {
            ItemName = item.Name,
            Price = item.Price
        }).ToList(),
        TotalOrderAmount = totalOrderAmount
    };

    return Results.Ok(orderDTO);
});


app.MapPost("/api/orders", async (WangazonDbContext db, CreateOrderDTO orderDTO) =>
{
    var revenueId = await db.Revenues
            .Where(r => r.EmployeeId == orderDTO.EmployeeId)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

    if (revenueId == 0)
    {
        return Results.NotFound("No associated revenue found for the employee.");
    }

    try
    {

        var order = new Order
        {
            EmployeeId = orderDTO.EmployeeId,
            OrderPlaced = DateTime.Now,
            OrderClosed = null,
            Tip = orderDTO.Tip,
            CustomerFirstName = orderDTO.CustomerFirstName,
            CustomerLastName = orderDTO.CustomerLastName,
            CustomerPhone = orderDTO.CustomerPhone,
            CustomerEmail = orderDTO.CustomerEmail,
            Review = orderDTO.Review,
            RevenueId = revenueId,

        };


        db.Add(order);
        db.SaveChanges();
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.MapPut("/api/orders/{id}", async (WangazonDbContext db, int id, CreateOrderDTO updatedOrderDTO) =>
{
    var order = await db.Orders.FindAsync(id);

    var revenueId = await db.Revenues
            .Where(r => r.EmployeeId == updatedOrderDTO.EmployeeId)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();


    if (order == null)
    {
        return Results.NotFound();
    }

    try
    {
        order.EmployeeId = updatedOrderDTO.EmployeeId;
        order.OrderPlaced = updatedOrderDTO.OrderPlaced;
        order.OrderClosed = updatedOrderDTO.OrderClosed;
        order.CustomerFirstName = updatedOrderDTO.CustomerFirstName;
        order.CustomerLastName = updatedOrderDTO.CustomerLastName;
        order.Tip = updatedOrderDTO.Tip;
        order.CustomerPhone = updatedOrderDTO.CustomerPhone;
        order.CustomerEmail = updatedOrderDTO.CustomerEmail;
        order.Review = updatedOrderDTO.Review;
        order.RevenueId = revenueId;

        db.Update(order);
        db.SaveChanges();

        return Results.Ok(order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.MapDelete("/api/orders/{id}", async (WangazonDbContext db, int id) =>
{
    var order = await db.Orders.FindAsync(id);

    if (order == null)
    {
        return Results.NotFound();
    }

    try
    {
        db.Remove(order);
        db.SaveChanges();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});









// Endpoints for Employee
app.MapGet("/api/employees", async (WangazonDbContext db) =>
{
    var employees = await db.Employees.ToListAsync();

    if (employees == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(employees);
});



// Endpoints for Menu Items
app.MapGet("/api/menuitems", async (WangazonDbContext db) =>
{
    var menuItems = await db.MenuItems.ToListAsync();

    if (menuItems == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(menuItems);
});

app.MapPost("/api/order/menuitem/{orderId}", (WangazonDbContext db, int orderId, int[] itemIds) =>
{
    var order = db.Orders.SingleOrDefault(s => s.Id == orderId);
    var items = db.MenuItems.Where(g => itemIds.Contains(g.Id)).ToList();

    if (order.MenuItems == null)
    {
        order.MenuItems = new List<MenuItem>();
    }

    foreach (var item in items)
    {
        order.MenuItems.Add(item);
    }

    db.SaveChanges();
    return order;
});




// Endpoints for Order Types
app.MapGet("/api/ordertypes", async (WangazonDbContext db) =>
{
    var orderTypes = await db.OrderTypes.ToListAsync();

    if (orderTypes == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(orderTypes);
});





// Endpoints for Payment Types
app.MapGet("/api/payment", async (WangazonDbContext db) =>
{
    var paymentTypes = await db.PaymentTypes.ToListAsync();

    if (paymentTypes == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(paymentTypes);
});




// Total revenue for every order
app.MapGet("/api/revenue", async (WangazonDbContext db) =>
{
    var revenueDTOs = await db.Revenues
        .Include(r => r.Orders)
        .ThenInclude(o => o.PaymentTypes)
        .Include(r => r.Orders)
        .ThenInclude(o => o.Type)
        .Include(r => r.Orders)
        .ThenInclude(o => o.MenuItems)
        .Where(r => r.Orders.Any(o => o.OrderClosed != null && o.OrderClosed.HasValue))
        .ToListAsync();

    var revenueDTOList = revenueDTOs.Select(r =>
    {
        var totalOrderAmountWithTip = r.Orders.Sum(o => o.CalculateTotalPrice());
        var paymentType = string.Join(", ", r.Orders.SelectMany(o => o.PaymentTypes.Select(pt => pt.Type)));
        var orderTypes = string.Join(", ", r.Orders.SelectMany(o => o.Type.Select(ot => ot.Type)));
        var orderClosed = r.Orders.Max(o => o.OrderClosed);
        var walkInCount = r.Orders.Count(o => o.Type.Any(ot => ot.Type == "Walk In"));
        var callInCount = r.Orders.Count(o => o.Type.Any(ot => ot.Type == "Call In"));
        var cashCount = r.Orders.Count(o => o.PaymentTypes.Any(pt => pt.Type == "Cash"));
        var creditCardCount = r.Orders.Count(o => o.PaymentTypes.Any(pt => pt.Type == "Credit Card"));


        return new RevenueDTO
        {
            Tip = r.TotalTips,
            TotalOrderAmountWithTip = totalOrderAmountWithTip,
            PaymentType = paymentType,
            OrderTypes = orderTypes,
            OrderClosed = orderClosed,
            WalkInCount = walkInCount,
            CallInCount = callInCount,
            CashCount = cashCount,
            CreditCardCount = creditCardCount,
        };
    }).ToList();

    return Results.Ok(revenueDTOList);
});

// revenue for a single employee
app.MapGet("/api/revenue/{employeeId}", async (WangazonDbContext db, int employeeId) =>
{
    var revenueDTOs = await db.Revenues
        .Include(r => r.Orders)
        .ThenInclude(o => o.PaymentTypes)
        .Include(r => r.Orders)
        .ThenInclude(o => o.Type)
        .Include(r => r.Orders)
        .ThenInclude(o => o.MenuItems)
        .Where(r => r.EmployeeId == employeeId)
        .Where(r => r.Orders.Any(o => o.OrderClosed != null && o.OrderClosed.HasValue))
        .ToListAsync();

    var revenueDTOList = revenueDTOs.Select(r =>
    {
        var totalOrderAmountWithTip = r.Orders.Sum(o => o.CalculateTotalPrice());
        var paymentType = string.Join(", ", r.Orders.SelectMany(o => o.PaymentTypes.Select(pt => pt.Type)));
        var orderTypes = string.Join(", ", r.Orders.SelectMany(o => o.Type.Select(ot => ot.Type)));
        var orderClosed = r.Orders.Max(o => o.OrderClosed);
        var walkInCount = r.Orders.Count(o => o.Type.Any(ot => ot.Type == "Walk In"));
        var callInCount = r.Orders.Count(o => o.Type.Any(ot => ot.Type == "Call In"));
        var cashCount = r.Orders.Count(o => o.PaymentTypes.Any(pt => pt.Type == "Cash"));
        var creditCardCount = r.Orders.Count(o => o.PaymentTypes.Any(pt => pt.Type == "Credit Card"));


        return new RevenueDTO
        {
            Tip = r.TotalTips,
            TotalOrderAmountWithTip = totalOrderAmountWithTip,
            PaymentType = paymentType,
            OrderTypes = orderTypes,
            OrderClosed = orderClosed,
            WalkInCount = walkInCount,
            CallInCount = callInCount,
            CashCount = cashCount,
            CreditCardCount = creditCardCount,
        };
    }).ToList();

    return Results.Ok(revenueDTOList);
});


// Check if user exists

app.MapGet("/api/checkuser/{uid}", (WangazonDbContext db, string uid) =>
{
    var userExists = db.Employees.Where(x => x.Uid == uid).FirstOrDefault();
    if (userExists == null)
    {
        return Results.StatusCode(204);
    }
    return Results.Ok(userExists);
});



app.Run();

