using Core.Interfaces;
using Models.Entities;
using System;

namespace DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(mainContext context) : base(context) { }
    }
}
