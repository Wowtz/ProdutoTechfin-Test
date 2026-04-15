using MediatR;
using ProdutoTechfin.Application.DTOs.Product;
using ProdutoTechfin.Domain.Repositories;

namespace ProdutoTechfin.Application.Queries.Products.GetAll
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public GetAllProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllAsync(cancellationToken);

            return products.Select(p => new ProductDto(
                p.Id,
                p.Name.Value,
                p.Description,
                p.Price.Amount,
                p.Price.Currency,
                p.StockQuantity.Value,
                p.IsActive,
                p.CreatedAt,
                p.UpdatedAt
            ));
        }
    }
}
