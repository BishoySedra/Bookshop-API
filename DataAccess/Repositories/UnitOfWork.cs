using Core.Interfaces;
using System;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly mainContext _context;

        public UnitOfWork(mainContext context)
        {
            _context = context;
            Categories = new CategoryRepository(context);
            Products = new ProductRepository(context);
        }

        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    }
}
