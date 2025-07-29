using Core.Interfaces;
using Core.Params;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(mainContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetPagedCategoriesAsync(PaginationParams paginationParams)
        {
            // Cast base DbContext (_context) to mainContext to access Categories DbSet
            var context = _context as mainContext;

            return await context.Categories
                .OrderBy(c => c.catOrder)
                .ThenBy(c => c.catName)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllOrderedAsync()
        {
            // Cast base DbContext (_context) to mainContext to access Categories DbSet
            var context = _context as mainContext;

            return await context.Categories
                .OrderBy(c => c.catOrder)
                .ThenBy(c => c.catName)
                .ToListAsync();
        }
    }
}
