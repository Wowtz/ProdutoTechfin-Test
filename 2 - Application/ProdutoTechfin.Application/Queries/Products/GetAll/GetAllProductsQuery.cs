using MediatR;
using ProdutoTechfin.Application.DTOs.Product;

namespace ProdutoTechfin.Application.Queries.Products.GetAll
{
    public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

}
