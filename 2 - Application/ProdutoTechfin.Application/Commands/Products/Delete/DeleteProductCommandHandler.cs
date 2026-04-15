using MediatR;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Exceptions;
using ProdutoTechfin.Domain.Repositories;

namespace ProdutoTechfin.Application.Commands.Products.Delete
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductEventPublisher _eventPublisher;

        public DeleteProductCommandHandler(
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            IProductEventPublisher eventPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new ProductNotFoundException(command.Id);

            product.Deactivate();

            _repository.Delete(product);
            await _unitOfWork.CommitAsync(cancellationToken);

            foreach (var domainEvent in product.DomainEvents)
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            product.ClearDomainEvents();
        }
    }
}
