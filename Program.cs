using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using wangazon;
using wangazon.DTOs;

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
        var orderClosed = r.Orders.Max(o => o.OrderClosed.Value);
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
        var orderClosed = r.Orders.Max(o => o.OrderClosed.Value);
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






app.Run();

