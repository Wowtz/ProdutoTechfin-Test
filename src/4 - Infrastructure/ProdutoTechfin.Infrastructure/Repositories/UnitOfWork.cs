using ProdutoTechfin.Domain.Repositories;
using ProdutoTechfin.Infrastructure.Persistence;
using System.Diagnostics.CodeAnalysis;

namespace ProdutoTechfin.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
