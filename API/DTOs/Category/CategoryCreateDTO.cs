using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Category
{
    public class CategoryCreateDTO
    {
        [Required]
        [MaxLength(50)]
        public string catName { get; set; }

        [Required]
        public int catOrder { get; set; }
    }
}
