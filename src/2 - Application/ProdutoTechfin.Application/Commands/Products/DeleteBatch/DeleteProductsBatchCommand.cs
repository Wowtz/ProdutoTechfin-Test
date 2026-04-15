using MediatR;

namespace ProdutoTechfin.Application.Commands.Products.DeleteBatch
{
    public record DeleteProductsBatchCommand(IEnumerable<Guid> Ids) : IRequest<DeleteProductsBatchResult>;

}
