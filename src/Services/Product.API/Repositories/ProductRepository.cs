using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Product.API.Entities;
using Product.API.Persistences;
using Product.API.Repositories.Interfaces;

namespace Product.API.Repositories
{
    public class ProductRepository : RepositoryBaseAsync<CatalogProduct, long, CatalogProductContext>, IProductRepository
    {
        public ProductRepository(CatalogProductContext dbContext, IUnitOfWork<CatalogProductContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public Task CreateProduct(CatalogProduct product) => CreateAsync(product);

        public async Task DeleteProduct(long id)
        {
            var product = await GetProduct(id);
            await DeleteAsync(product);
        }

        public async Task<CatalogProduct> GetProduct(long id) => await GetByIdAsync(id);

        public Task<CatalogProduct> GetProductByNo(string productNo) =>
            FindByCondition(x => x.No.Equals(productNo)).SingleOrDefaultAsync();

        public async Task<IEnumerable<CatalogProduct>> GetProducts() => await FindAll().ToListAsync();

        public Task UpdateProduct(CatalogProduct product)=> UpdateAsync(product);
    }
}
