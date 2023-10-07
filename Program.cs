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
            OrderType = orderTypes,
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

