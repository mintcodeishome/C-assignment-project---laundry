using Microsoft.EntityFrameworkCore;
using Laundry.Data;
using Laundry.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<LaundryDbContext>(options =>
    options.UseSqlite("Data Source=laundry.db"));

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LaundryDbContext>();
    dbContext.Database.EnsureCreated();
}

// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/submit", async (HttpContext context, LaundryDbContext dbContext) =>
{
    var form = await context.Request.ReadFormAsync();

    string name = form["name"]!;
    string phone = form["phone"]!;

    int shirts = int.Parse(form["shirts"]!);
    int trousers = int.Parse(form["trousers"]!);
    int suits = int.Parse(form["suits"]!);
    int bedsheets = int.Parse(form["bedsheets"]!);

    // Prices
    decimal priceShirts = shirts * 50;
    decimal priceTrousers = trousers * 70;
    decimal priceSuits = suits * 150;
    decimal priceBedsheets = bedsheets * 100;

    decimal total = priceShirts + priceTrousers + priceSuits + priceBedsheets;

    // Create order
    var order = new LaundryOrder
    {
        CustomerName = name,
        PhoneNumber = phone,
        TotalAmount = total
    };

    // Add items
    if (shirts > 0)
    {
        order.Items.Add(new OrderItem
        {
            ItemType = "Shirt",
            Quantity = shirts,
            PricePerUnit = 50,
            TotalPrice = priceShirts
        });
    }

    if (trousers > 0)
    {
        order.Items.Add(new OrderItem
        {
            ItemType = "Trousers",
            Quantity = trousers,
            PricePerUnit = 70,
            TotalPrice = priceTrousers
        });
    }

    if (suits > 0)
    {
        order.Items.Add(new OrderItem
        {
            ItemType = "Suit",
            Quantity = suits,
            PricePerUnit = 150,
            TotalPrice = priceSuits
        });
    }

    if (bedsheets > 0)
    {
        order.Items.Add(new OrderItem
        {
            ItemType = "Bedsheet",
            Quantity = bedsheets,
            PricePerUnit = 100,
            TotalPrice = priceBedsheets
        });
    }

    // Save to database
    dbContext.LaundryOrders.Add(order);
    await dbContext.SaveChangesAsync();

    // Generate receipt HTML
    return Results.Content(GenerateReceiptHtml(order), "text/html");
});

string GenerateReceiptHtml(LaundryOrder order)
{
    var itemsHtml = string.Join("", order.Items.Select(item => $@"
        <div class='order-item'>
            <span>{item.ItemType} ({item.Quantity} × {item.PricePerUnit} KES)</span>
            <span>{item.TotalPrice} KES</span>
        </div>
    "));

    return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Receipt - TeamSafi Laundry</title>
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            padding: 20px;
            margin: 0;
        }}
        
        .receipt-container {{
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 500px;
        }}
        
        .receipt-header {{
            text-align: center;
            margin-bottom: 20px;
            border-bottom: 2px solid #3498db;
            padding-bottom: 15px;
        }}
        
        .success-icon {{
            color: #2ecc71;
            font-size: 3rem;
            margin-bottom: 10px;
        }}
        
        h2 {{
            color: #3498db;
            margin: 0 0 5px 0;
        }}
        
        .order-id {{
            background: #f8f9fa;
            padding: 8px 15px;
            border-radius: 20px;
            font-weight: bold;
            color: #6c757d;
            display: inline-block;
            margin-top: 10px;
        }}
        
        .receipt-details {{
            background: #f8f9fa;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
        }}
        
        .customer-info {{
            margin-bottom: 20px;
        }}
        
        .customer-info p {{
            margin: 5px 0;
        }}
        
        .order-item {{
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
            padding-bottom: 10px;
            border-bottom: 1px solid #e9ecef;
        }}
        
        .order-total {{
            display: flex;
            justify-content: space-between;
            font-weight: bold;
            font-size: 1.2rem;
            margin-top: 15px;
            padding-top: 15px;
            border-top: 2px dashed #e9ecef;
            color: #3498db;
        }}
        
        .receipt-footer {{
            text-align: center;
            margin-top: 20px;
            color: #6c757d;
            font-size: 0.9rem;
        }}
        
        .action-buttons {{
            display: flex;
            gap: 10px;
            margin-top: 20px;
        }}
        
        .btn {{
            display: inline-block;
            padding: 12px 20px;
            border-radius: 6px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.3s ease;
            text-align: center;
            flex: 1;
        }}
        
        .btn-primary {{
            background: #3498db;
            color: white;
        }}
        
        .btn-primary:hover {{
            background: #2980b9;
            transform: translateY(-2px);
        }}
        
        .btn-secondary {{
            background: #f8f9fa;
            color: #343a40;
            border: 1px solid #e9ecef;
        }}
        
        .btn-secondary:hover {{
            background: #e9ecef;
            transform: translateY(-2px);
        }}
        
        @media print {{
            body {{
                background: white;
                padding: 0;
            }}
            
            .receipt-container {{
                box-shadow: none;
                padding: 15px;
            }}
            
            .action-buttons {{
                display: none;
            }}
        }}
    </style>
</head>
<body>
    <div class=""receipt-container"">
        <div class=""receipt-header"">
            <div class=""success-icon"">
                <i class=""fas fa-check-circle""></i>
            </div>
            <h2>✅ Order Received Successfully!</h2>
            <div class=""order-id"">Order #: {order.OrderId}</div>
        </div>
        
        <div class=""receipt-details"">
            <div class=""customer-info"">
                <h3>Customer Information</h3>
                <p><strong>Name:</strong> {order.CustomerName}</p>
                <p><strong>Phone:</strong> {order.PhoneNumber}</p>
                <p><strong>Order Date:</strong> {order.OrderDate:yyyy-MM-dd HH:mm}</p>
            </div>
            
            <h3>Order Summary</h3>
            {itemsHtml}
            
            <div class=""order-total"">
                <span>Total Amount:</span>
                <span>{order.TotalAmount} KES</span>
            </div>
        </div>
        
        <div class=""receipt-footer"">
            <p>Thank you for choosing TeamSafi Laundry! Your clothes will be ready soon.</p>
            <p><i class=""fas fa-phone""></i> Contact us: +254 700 123 456</p>
        </div>
        
        <div class=""action-buttons"">
            <a href=""/"" class=""btn btn-primary"">
                <i class=""fas fa-arrow-left""></i> Place Another Order
            </a>
            <button onclick=""window.print()"" class=""btn btn-secondary"">
                <i class=""fas fa-print""></i> Print Receipt
            </button>
        </div>
    </div>
</body>
</html>";
}

app.Run();