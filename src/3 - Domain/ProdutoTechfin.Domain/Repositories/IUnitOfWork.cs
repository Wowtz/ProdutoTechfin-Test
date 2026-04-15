using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
