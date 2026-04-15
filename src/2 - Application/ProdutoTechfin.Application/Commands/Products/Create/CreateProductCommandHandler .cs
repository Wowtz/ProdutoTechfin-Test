using MediatR;
using ProdutoTechfin.Application.DTOs.Product;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.Exceptions;
using ProdutoTechfin.Domain.Repositories;

namespace ProdutoTechfin.Application.Commands.Products.Create
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductEventPublisher _eventPublisher;

        public CreateProductCommandHandler(
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            IProductEventPublisher eventPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var nameExists = await _repository.ExistsByNameAsync(command.Name, cancellationToken: cancellationToken);
            if (nameExists)
                throw new DuplicateProductNameException(command.Name);

            var product = Product.Create(
                command.Name,
                command.Description,
                command.Price,
                command.StockQuantity
            );

            await _repository.AddAsync(product, cancellationToken);
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
