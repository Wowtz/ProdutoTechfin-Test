namespace ProdutoTechfin.Domain.Events
{
    public record ProductCreatedEvent(Guid ProductId, string ProductName)
    : DomainEvent(ProductId);
}
