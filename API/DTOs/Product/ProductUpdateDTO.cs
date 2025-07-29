using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Product
{
    public class ProductUpdateDTO
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Author { get; set; }

        [Required]
        [Range(1, 1000)]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
