using Microsoft.EntityFrameworkCore;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.Repositories;
using ProdutoTechfin.Infrastructure.Persistence;

namespace ProdutoTechfin.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Name.Value)
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var normalizedName = name.ToLower().Trim();

            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return products.Any(p =>
                p.Name.Value.ToLower() == normalizedName &&
                (excludeId == null || p.Id != excludeId));
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public void DeleteRange(IEnumerable<Product> products)
        {
            _context.Products.RemoveRange(products);
        }
    }
}
