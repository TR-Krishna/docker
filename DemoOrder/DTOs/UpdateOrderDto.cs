namespace DemoOrder.DTOs
{
    public class UpdateOrderDto
    {
        public string Status { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
