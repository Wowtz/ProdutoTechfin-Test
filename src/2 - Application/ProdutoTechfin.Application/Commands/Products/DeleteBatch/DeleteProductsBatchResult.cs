namespace ProdutoTechfin.Application.Commands.Products.DeleteBatch
{
    public record DeleteProductsBatchResult(
     int TotalRequested,
     int TotalDeleted,
     IEnumerable<Guid> NotFoundIds);
}
