using MediatR;
using ProdutoTechfin.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.Application.Queries.Products.GetById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
}
