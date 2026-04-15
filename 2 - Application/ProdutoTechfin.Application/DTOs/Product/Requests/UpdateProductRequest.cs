namespace ProdutoTechfin.Application.DTOs.Product.Requests
{
    public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
);
}
