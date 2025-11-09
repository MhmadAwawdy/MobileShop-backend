namespace TechApi.Models
{
    public class BillDto
    {
        public string BillNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<BillItemDto> BillItems { get; set; } = new List<BillItemDto>();
    }

    public class BillItemDto
    {
        public int BillItemId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
