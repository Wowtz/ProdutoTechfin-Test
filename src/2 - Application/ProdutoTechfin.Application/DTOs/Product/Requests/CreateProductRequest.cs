namespace ProdutoTechfin.Application.DTOs.Product.Requests
{
    public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
);
}
