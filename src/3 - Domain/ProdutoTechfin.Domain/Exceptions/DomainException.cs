using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class ProductNotFoundException : DomainException
    {
        public ProductNotFoundException(Guid id)
            : base($"Product with id '{id}' was not found.") { }
    }

    public class DuplicateProductNameException : DomainException
    {
        public DuplicateProductNameException(string name)
            : base($"A product with the name '{name}' already exists.") { }
    }
}
