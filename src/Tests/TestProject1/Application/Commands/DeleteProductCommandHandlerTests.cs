using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProdutoTechfin.Application.Commands.Products.Delete;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.Events;
using ProdutoTechfin.Domain.Exceptions;
using ProdutoTechfin.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutoTechfin.UnitTests.Application.Commands
{

    public class DeleteProductCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductEventPublisher> _eventPublisherMock;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
            _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
            _eventPublisherMock = _fixture.Freeze<Mock<IProductEventPublisher>>();

            _handler = new DeleteProductCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _eventPublisherMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProductExists_ShouldCallDeleteAndCommit()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await _handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProductExists_ShouldDeactivateProduct()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await _handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

            // Assert
            product.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_WhenProductExists_ShouldPublishProductDeletedEvent()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await _handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

            // Assert
            _eventPublisherMock.Verify(
                e => e.PublishAsync(It.IsAny<ProductDeletedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
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
            var act = async () => await _handler.Handle(new DeleteProductCommand(id), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ProductNotFoundException>();
        }
    }
}
