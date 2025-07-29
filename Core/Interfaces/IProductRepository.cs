using Models.Entities;

namespace Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Add custom Product-specific methods here if needed
    }
}
