namespace ProdutoTechfin.Application.DTOs.Product.Requests
{
    public record DeleteProductsBatchRequest(
    IEnumerable<Guid> Ids);
}
