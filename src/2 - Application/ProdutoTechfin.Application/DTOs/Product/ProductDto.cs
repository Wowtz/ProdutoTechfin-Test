namespace ProdutoTechfin.Application.DTOs.Product
{
    public record ProductDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        int StockQuantity,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
