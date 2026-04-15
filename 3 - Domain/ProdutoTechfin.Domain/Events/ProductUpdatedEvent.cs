namespace ProdutoTechfin.Domain.Events
{
    public record ProductUpdatedEvent(Guid ProductId, string ProductName)
    : DomainEvent(ProductId);
}
