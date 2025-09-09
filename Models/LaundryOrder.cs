namespace Laundry.Models;

public class LaundryOrder
{
    public int Id { get; set; }
    public string OrderId { get; set; } = GenerateOrderId();
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    private static string GenerateOrderId()
    {
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}

public class OrderItem
{
    public int Id { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal TotalPrice { get; set; }
    public int LaundryOrderId { get; set; }
    public LaundryOrder? LaundryOrder { get; set; }
}