using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using wangazon;
using wangazon.DTOs;
using wangazon.Models;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:7253")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

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

app.UseCors(MyAllowSpecificOrigins);

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
        Id = order.Id,
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
        .Include(o => o.OrderMenuItems)
        .ThenInclude(om => om.MenuItem)
        .Where(o => o.Id == id)
        .FirstOrDefaultAsync();

    if (order == null)
    {
        return Results.NotFound();
    }

    decimal totalOrderAmount = order.OrderMenuItems
        .Sum(orderItem => orderItem.Quantity * orderItem.MenuItem.Price);

    var orderDTO = new OrderDTO
    {
        Id = order.Id,
        OrderName = order.CustomerFirstName + " " + order.CustomerLastName,
        OrderStatus = order.OrderClosed.HasValue ? "Closed" : "Open",
        PhoneNumber = order.CustomerPhone,
        EmailAddress = order.CustomerEmail,
        OrderType = order.Type?.FirstOrDefault()?.Type,
        MenuItems = order.OrderMenuItems.Select(orderItem => new MenuItemDTO
        {
            Id = orderItem.MenuItem.Id,
            ItemName = orderItem.MenuItem.Name,
            Price = orderItem.MenuItem.Price,
            Quantity = orderItem.Quantity,
            Comment = orderItem.Comment,
        }).ToList(),
        TotalOrderAmount = totalOrderAmount,
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
            Id = orderDTO.Id,
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
        order.OrderPlaced = updatedOrderDTO.OrderPlaced;
        order.OrderClosed = updatedOrderDTO.OrderClosed;
        order.CustomerFirstName = updatedOrderDTO.CustomerFirstName;
        order.CustomerLastName = updatedOrderDTO.CustomerLastName;
        order.Tip = updatedOrderDTO.Tip;
        order.CustomerPhone = updatedOrderDTO.CustomerPhone;
        order.CustomerEmail = updatedOrderDTO.CustomerEmail;
        order.Review = updatedOrderDTO.Review;

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

app.MapGet("/api/employees/{empId}", async (WangazonDbContext db, int id) =>
{
    Employee emp = await db.Employees.FirstOrDefaultAsync(e => e.Id == id);

    if (emp == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(emp);
});

app.MapPut("/api/employees/{empId}", async (WangazonDbContext db, int id, Employee emp) =>
{
    Employee empToUpdate = await db.Employees.FirstOrDefaultAsync(e => e.Id == id);

    if (empToUpdate == null)
    {
        return Results.NotFound();
    }

    empToUpdate.FirstName = emp.FirstName;
    empToUpdate.LastName = emp.LastName;
    empToUpdate.Email = emp.Email;
    empToUpdate.Uid = emp.Uid;

    db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/employees/{empId}", async (WangazonDbContext db, int id) =>
{
    Employee emp = await db.Employees.FirstOrDefaultAsync(e => e.Id == id);

    if (emp == null)
    {
        return Results.NotFound();
    }

    db.Remove(emp);
    db.SaveChangesAsync();
    return Results.NoContent();
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

app.MapGet("/api/menuitem/{id}", async (WangazonDbContext db, int id) =>
{
    var menuItem = await db.MenuItems.FirstOrDefaultAsync(i => i.Id == id);

    if (menuItem == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(menuItem);
});

// add item to order
app.MapPost("/api/order/menuitem/{orderId}/{itemId}", (WangazonDbContext db, int orderId, int itemId) =>
{
    var orderItem = db.OrderMenuItems
        .SingleOrDefault(oi => oi.OrderId == orderId && oi.MenuItemId == itemId);

    if (orderItem == null)
    {

        orderItem = new OrderMenuItem
        {
            OrderId = orderId,
            MenuItemId = itemId,
            Quantity = 1
        };

        db.OrderMenuItems.Add(orderItem);
    }
    else
    {

        orderItem.Quantity++;
    }

    db.SaveChanges();
    return orderItem;
});

app.MapPut("/api/order/menuitem/{orderId}/{itemId}", (WangazonDbContext db, int orderId, int itemId, MenuItemDTO updatedMenuItem) =>
{
    var orderItem = db.OrderMenuItems
        .SingleOrDefault(oi => oi.OrderId == orderId && oi.MenuItemId == itemId);

    if (orderItem == null)
    {
        return Results.NotFound("Order menu item not found.");
    }

    orderItem.Comment = updatedMenuItem.Comment;

    try
    {
        db.SaveChanges();
        return Results.Ok(orderItem);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});



// delete item from order
app.MapDelete("/api/order/menuitem/{orderId}/{itemId}", async (WangazonDbContext db, int orderId, int itemId) =>
{
    var order = await db.Orders
        .Include(o => o.OrderMenuItems)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    if (order == null)
    {
        return Results.NotFound();
    }

    var orderItem = order.OrderMenuItems.FirstOrDefault(oi => oi.MenuItemId == itemId);

    if (orderItem == null)
    {
        return Results.NoContent();
    }

    if (orderItem.Quantity <= 1)
    {
        db.OrderMenuItems.Remove(orderItem);
    }
    else
    {
        orderItem.Quantity--;
    }

    await db.SaveChangesAsync();

    return Results.NoContent();
});






// create menu item
app.MapPost("/api/menuitem", (WangazonDbContext db, MenuItem item) =>
{
    try
    {
        db.Add(item);
        db.SaveChanges();
        return Results.Created($"/api/menuitem/{item.Id}", item);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

// edit menu item
app.MapPut("/api/menuitem/{id}", async (WangazonDbContext db, int id, MenuItem item) =>
{
    var itemToUpdate = await db.MenuItems.FirstOrDefaultAsync(i => i.Id == id);
    if (itemToUpdate == null)
    {
        return Results.NotFound();
    }

    itemToUpdate.Description = item.Description;
    itemToUpdate.ImageUrl = item.ImageUrl;
    itemToUpdate.Price = item.Price;
    itemToUpdate.Name = item.Name;

    db.SaveChanges();
    return Results.Ok(itemToUpdate);
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

app.MapPost("/api/order/ordertype/{orderId}/{typeId}", async (WangazonDbContext db, int orderId, int typeId) =>
{
    var order = await db.Orders
        .Include(o => o.Type)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    var orderType = await db.OrderTypes.FindAsync(typeId);

    if (order == null || orderType == null)
    {
        return Results.NotFound();
    }


    if (order.Type == null)
    {
        order.Type = new List<OrderType>();
    }

    order.Type.Add(orderType);

    try
    {
        db.Update(order);
        await db.SaveChangesAsync();
        return Results.Ok(order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.MapPut("/api/order/ordertype/{orderId}/{typeId}", async (WangazonDbContext db, int orderId, int typeId) =>
{
    var order = await db.Orders
        .Include(o => o.Type)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    var orderType = await db.OrderTypes.FindAsync(typeId);

    if (order == null || orderType == null)
    {
        return Results.NotFound();
    }

    if (order.Type == null)
    {
        order.Type = new List<OrderType>();
    }

    order.Type.Clear();
    order.Type.Add(orderType);

    try
    {
        db.Update(order);
        await db.SaveChangesAsync();
        return Results.Ok(order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
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

app.MapPost("/api/orders/{orderId}/payment", async (WangazonDbContext db, int orderId, PaymentDTO confirmPaymentDTO) =>
{
    var order = await db.Orders
        .Include(o => o.PaymentTypes)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    if (order == null)
    {
        return Results.NotFound();
    }

    try
    {

        var paymentType = await db.PaymentTypes.FirstOrDefaultAsync(pt => pt.Type == confirmPaymentDTO.PaymentType);

        if (paymentType == null)
        {
            return Results.BadRequest("Invalid payment type");
        }

        order.Tip = confirmPaymentDTO.Tip;
        order.PaymentTypes.Add(paymentType);
        order.OrderClosed = DateTime.Now;

        db.Update(order);
        await db.SaveChangesAsync();

        return Results.Ok(order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
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

