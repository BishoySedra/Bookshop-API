using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Category
{
    public class CategoryUpdateDTO
    {
        [Required]
        [MaxLength(50)]
        public string catName { get; set; }

        [Required]
        public int catOrder { get; set; }

        public bool markedAsDeleted { get; set; }
    }
}
