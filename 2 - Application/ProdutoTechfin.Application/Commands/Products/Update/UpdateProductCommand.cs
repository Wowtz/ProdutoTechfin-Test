using MediatR;
using ProdutoTechfin.Application.DTOs.Product;

namespace ProdutoTechfin.Application.Commands.Products.Update
{
    public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
) : IRequest<ProductDto>;
}
