using MediatR;
using ProdutoTechfin.Application.DTOs.Product;
using ProdutoTechfin.Domain.Exceptions;
using ProdutoTechfin.Domain.Repositories;

namespace ProdutoTechfin.Application.Queries.Products.GetById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(query.Id, cancellationToken)
                ?? throw new ProductNotFoundException(query.Id);

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
