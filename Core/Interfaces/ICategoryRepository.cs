using Core.Params;
using Models.Entities;

namespace Core.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Add custom Category-specific methods here if needed
        Task<IEnumerable<Category>> GetPagedCategoriesAsync(PaginationParams paginationParams);

    }
}
