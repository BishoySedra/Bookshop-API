namespace Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; } // Nullable without using `?`

        public string Author { get; set; }

        public decimal Price { get; set; }

        // Added this for the relationship for one-to-many with Category
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}