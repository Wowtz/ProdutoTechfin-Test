using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProdutoTechfin.Application.Queries.Products.GetById;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.Exceptions;
using ProdutoTechfin.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.UnitTests.Application.Queries
{
    public class GetProductByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly GetProductByIdQueryHandler _handler;

        public GetProductByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
            _handler = new GetProductByIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProductExists_ShouldReturnMappedDto()
        {
            // Arrange
            var product = Product.Create("Notebook Pro", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(product.Id);
            result.Name.Should().Be(product.Name.Value);
            result.Price.Should().Be(product.Price.Amount);
            result.StockQuantity.Should().Be(product.StockQuantity.Value);
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WhenProductNotFound_ShouldThrowProductNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _handler.Handle(new GetProductByIdQuery(id), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ProductNotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnce()
        {
            // Arrange
            var product = Product.Create("Notebook Pro", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await _handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

            // Assert
            _repositoryMock.Verify(
                r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
