namespace TechApi.Models
{
    public class BillRequestDto
    {
        public int CustomerId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TaxPercentage { get; set; }
        public List<BillItemRequestDto> BillItems { get; set; } = new List<BillItemRequestDto>();
    }

    public class BillItemRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}