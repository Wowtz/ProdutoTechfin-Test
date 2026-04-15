using ProdutoTechfin.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.Domain.ValueObjects
{
    public record ProductName
    {
        public static readonly int MaxLength = 200;
        public string Value { get; }

        public ProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Product name is required.");

            if (value.Trim().Length > MaxLength)
                throw new DomainException($"Product name cannot exceed {MaxLength} characters.");

            Value = value.Trim();
        }

        public override string ToString() => Value;
    }
}
