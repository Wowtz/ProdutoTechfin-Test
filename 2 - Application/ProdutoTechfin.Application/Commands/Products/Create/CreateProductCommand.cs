using ProdutoTechfin.Application.DTOs.Product;
using MediatR;

namespace ProdutoTechfin.Application.Commands.Products.Create
{
    public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
) : IRequest<ProductDto>;
}
