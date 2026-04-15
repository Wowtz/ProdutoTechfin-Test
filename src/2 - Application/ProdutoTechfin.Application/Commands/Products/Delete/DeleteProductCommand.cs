using MediatR;

namespace ProdutoTechfin.Application.Commands.Products.Delete
{
    public record DeleteProductCommand(Guid Id) : IRequest;

}
