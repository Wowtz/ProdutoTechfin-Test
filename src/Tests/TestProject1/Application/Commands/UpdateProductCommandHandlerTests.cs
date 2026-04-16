using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProdutoTechfin.Application.Commands.Products.Update;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.Events;
using ProdutoTechfin.Domain.Exceptions;
using ProdutoTechfin.Domain.Repositories;

namespace ProdutoTechfin.UnitTests.Application.Commands
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductEventPublisher> _eventPublisherMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
            _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
            _eventPublisherMock = _fixture.Freeze<Mock<IProductEventPublisher>>();

            _handler = new UpdateProductCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _eventPublisherMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnUpdatedDto()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 1000m, 10);
            var command = new UpdateProductCommand(product.Id, "Notebook Pro", "nova desc", 2000m, 20);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(command.Name);
            result.Price.Should().Be(command.Price);
            result.StockQuantity.Should().Be(command.StockQuantity);
            result.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCallUpdateAndCommit()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 1000m, 10);
            var command = new UpdateProductCommand(product.Id, "Notebook Pro", "nova desc", 2000m, 20);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldPublishProductUpdatedEvent()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 1000m, 10);
            var command = new UpdateProductCommand(product.Id, "Notebook Pro", "nova desc", 2000m, 20);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _eventPublisherMock.Verify(
                e => e.PublishAsync(It.IsAny<ProductUpdatedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProductNotFound_ShouldThrowProductNotFoundException()
        {
            // Arrange
            var command = new UpdateProductCommand(Guid.NewGuid(), "Notebook Pro", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ProductNotFoundException>();
        }

        [Fact]
        public async Task Handle_WithDuplicateName_ShouldThrowDuplicateProductNameException()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 1000m, 10);
            var command = new UpdateProductCommand(product.Id, "Notebook Pro", "desc", 1000m, 10);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DuplicateProductNameException>();
        }
    }
}
