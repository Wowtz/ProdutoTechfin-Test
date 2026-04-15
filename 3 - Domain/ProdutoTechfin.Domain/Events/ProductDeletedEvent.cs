namespace ProdutoTechfin.Domain.Events
{
    public record ProductDeletedEvent(Guid ProductId, string ProductName)
    : DomainEvent(ProductId);
}
