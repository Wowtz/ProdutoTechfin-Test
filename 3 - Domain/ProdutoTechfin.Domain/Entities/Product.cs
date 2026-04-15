using ProdutoTechfin.Domain.Events;
using ProdutoTechfin.Domain.ValueObjects;

namespace ProdutoTechfin.Domain.Entities
{
    public class Product
    {
        private readonly List<DomainEvent> _domainEvents = new();

        public Guid Id { get; private set; }
        public ProductName Name { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = null!;
        public StockQuantity StockQuantity { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private Product() { }

        public static Product Create(string name, string description, decimal price, int stockQuantity)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = new ProductName(name),
                Description = description?.Trim() ?? string.Empty,
                Price = new Money(price),
                StockQuantity = new StockQuantity(stockQuantity),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name.Value));

            return product;
        }

        public void Update(string name, string description, decimal price, int stockQuantity)
        {
            Name = new ProductName(name);
            Description = description?.Trim() ?? string.Empty;
            Price = new Money(price);
            StockQuantity = new StockQuantity(stockQuantity);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ProductUpdatedEvent(Id, Name.Value));
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ProductDeletedEvent(Id, Name.Value));
        }

        public void ClearDomainEvents() => _domainEvents.Clear();

        private void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    }
}
