using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProdutoTechfin.Application.Commands.Products.Create;
using ProdutoTechfin.Application.Commands.Products.Delete;
using ProdutoTechfin.Application.Commands.Products.DeleteBatch;
using ProdutoTechfin.Application.Commands.Products.Update;
using ProdutoTechfin.Application.DTOs.Product;
using ProdutoTechfin.Application.Queries.Products.GetAll;
using ProdutoTechfin.Application.Queries.Products.GetById;

namespace ProdutoTechfin.Api.Controllers;

using Asp.Versioning;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class ProductsController : MainController<ProductsController>
{
    private readonly IMediator _mediator;

    public ProductsController(
        IMediator mediator,
        ILogger<MainController<ProductsController>> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retorna todos os produtos cadastrados.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        LogServiceCall(nameof(IMediator), nameof(IMediator.Send));
        var result = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retorna um produto pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        LogServiceCall(nameof(IMediator), nameof(IMediator.Send), id); // ← faltou esse
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
    [FromBody] CreateProductCommand command,
    CancellationToken cancellationToken)
    {
        LogServiceCall(nameof(IMediator), nameof(IMediator.Send), command);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateProductCommand command,
    CancellationToken cancellationToken)
    {
        LogServiceCall(nameof(IMediator), nameof(IMediator.Send), command);
        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Remove um produto pelo ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        LogServiceCall(nameof(IMediator), nameof(IMediator.Send), id);
        await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove múltiplos produtos em lote.
    /// </summary>
    [HttpDelete("batch")]
    [ProducesResponseType(typeof(DeleteProductsBatchResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBatch(
    [FromBody] DeleteProductsBatchCommand command,
    CancellationToken cancellationToken)
    {
        LogServiceCall(nameof(IMediator), nameof(IMediator.Send), command);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}