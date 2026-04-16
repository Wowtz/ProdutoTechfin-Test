using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProdutoTechfin.Application.Queries.Products.GetAll;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.UnitTests.Application.Queries
{
    public class GetAllProductsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly GetAllProductsQueryHandler _handler;

        public GetAllProductsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
            _handler = new GetAllProductsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProductsExist_ShouldReturnMappedDtos()
        {
            // Arrange
            var products = new List<Product>
        {
            Product.Create("Notebook Pro", "desc", 1000m, 10),
            Product.Create("Mouse Gamer", "desc", 250m, 20),
            Product.Create("Teclado TKL", "desc", 350m, 15)
        };

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(p => p.Name).Should().BeEquivalentTo(
                products.Select(p => p.Name.Value));
        }

        [Fact]
        public async Task Handle_WhenNoProductsExist_ShouldReturnEmptyList()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnce()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            // Act
            await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            // Assert
            _repositoryMock.Verify(
                r => r.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapProductDtoCorrectly()
        {
            // Arrange
            var product = Product.Create("Notebook Pro", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            // Act
            var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);
            var dto = result.First();

            // Assert
            dto.Id.Should().Be(product.Id);
            dto.Name.Should().Be(product.Name.Value);
            dto.Description.Should().Be(product.Description);
            dto.Price.Should().Be(product.Price.Amount);
            dto.Currency.Should().Be(product.Price.Currency);
            dto.StockQuantity.Should().Be(product.StockQuantity.Value);
            dto.IsActive.Should().BeTrue();
        }
    }
}
