namespace API.DTOs.Product
{
    public class ProductPatchDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
