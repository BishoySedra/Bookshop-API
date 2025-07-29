using Core.Interfaces;
using Models.Entities;
using System;

namespace DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(mainContext context) : base(context) { }
    }
}
