using ProdutoTechfin.Domain.Exceptions;

namespace ProdutoTechfin.Domain.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "BRL")
        {
            if (amount < 0)
                throw new DomainException("Amount cannot be negative.");

            Amount = amount;
            Currency = currency;
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException("Cannot add different currencies.");

            return new Money(Amount + other.Amount, Currency);
        }

        public override string ToString() => $"{Currency} {Amount:N2}";
    }
}
