using MediatR;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Repositories;

namespace ProdutoTechfin.Application.Commands.Products.DeleteBatch
{
    public class DeleteProductsBatchCommandHandler : IRequestHandler<DeleteProductsBatchCommand, DeleteProductsBatchResult>
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductEventPublisher _eventPublisher;

        public DeleteProductsBatchCommandHandler(
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            IProductEventPublisher eventPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<DeleteProductsBatchResult> Handle(DeleteProductsBatchCommand command, CancellationToken cancellationToken)
        {
            var ids = command.Ids.Distinct().ToList();

            var products = (await _repository.GetByIdsAsync(ids, cancellationToken)).ToList();

            var foundIds = products.Select(p => p.Id).ToHashSet();
            var notFoundIds = ids.Where(id => !foundIds.Contains(id)).ToList();

            foreach (var product in products)
                product.Deactivate();

            _repository.DeleteRange(products);
            await _unitOfWork.CommitAsync(cancellationToken);

            var domainEvents = products.SelectMany(p => p.DomainEvents).ToList();

            foreach (var domainEvent in domainEvents)
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            foreach (var product in products)
                product.ClearDomainEvents();

            return new DeleteProductsBatchResult(
                TotalRequested: ids.Count,
                TotalDeleted: products.Count,
                NotFoundIds: notFoundIds
            );
        }
    }
}
