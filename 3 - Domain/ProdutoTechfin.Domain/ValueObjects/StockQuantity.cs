using ProdutoTechfin.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.Domain.ValueObjects
{
    public record StockQuantity
    {
        public int Value { get; }

        public StockQuantity(int value)
        {
            if (value < 0)
                throw new DomainException("Stock quantity cannot be negative.");

            Value = value;
        }

        public StockQuantity Add(int quantity) => new(Value + quantity);

        public StockQuantity Remove(int quantity)
        {
            if (quantity > Value)
                throw new DomainException("Insufficient stock.");

            return new(Value - quantity);
        }

        public override string ToString() => Value.ToString();
    }
}
