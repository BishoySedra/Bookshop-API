namespace API.DTOs.Category
{
    public class CategoryReadDTO
    {
        public int Id { get; set; }
        public string catName { get; set; }
        public int catOrder { get; set; }
        public bool markedAsDeleted { get; set; }
    }
}
