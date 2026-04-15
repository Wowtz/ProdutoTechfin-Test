using MediatR;
using ProdutoTechfin.Application.DTOs.Product;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Exceptions;
using ProdutoTechfin.Domain.Repositories;

namespace ProdutoTechfin.Application.Commands.Products.Update
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductEventPublisher _eventPublisher;

        public UpdateProductCommandHandler(
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            IProductEventPublisher eventPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new ProductNotFoundException(command.Id);

            var nameExists = await _repository.ExistsByNameAsync(
                command.Name,
                excludeId: command.Id,
                cancellationToken: cancellationToken);

            if (nameExists)
                throw new DuplicateProductNameException(command.Name);

            product.Update(
                command.Name,
                command.Description,
                command.Price,
                command.StockQuantity
            );

            _repository.Update(product);
            await _unitOfWork.CommitAsync(cancellationToken);

            foreach (var domainEvent in product.DomainEvents)
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            product.ClearDomainEvents();

            return new ProductDto(
                product.Id,
                product.Name.Value,
                product.Description,
                product.Price.Amount,
                product.Price.Currency,
                product.StockQuantity.Value,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt
            );
        }
    }
}
