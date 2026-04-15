using ProdutoTechfin.Domain.Events;

namespace ProdutoTechfin.Application.Interfaces.Events
{
    public interface IProductEventPublisher
    {
        Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
