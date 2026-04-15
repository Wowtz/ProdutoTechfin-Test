namespace ProdutoTechfin.Domain.Events
{
    public abstract record DomainEvent(Guid Id, DateTime OccurredAt)
    {
        protected DomainEvent(Guid id) : this(id, DateTime.UtcNow) { }
    }
}
