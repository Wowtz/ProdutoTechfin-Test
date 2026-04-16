using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProdutoTechfin.Application.Commands.Products.Create;
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
    public class CreateProductCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductEventPublisher> _eventPublisherMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _repositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
            _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
            _eventPublisherMock = _fixture.Freeze<Mock<IProductEventPublisher>>();

            _handler = new CreateProductCommandHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _eventPublisherMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnProductDto()
        {
            // Arrange
            var command = _fixture.Build<CreateProductCommand>()
                .With(x => x.Name, "Notebook Pro")
                .With(x => x.Price, 1000m)
                .With(x => x.StockQuantity, 10)
                .Create();

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(command.Name);
            result.Price.Should().Be(command.Price);
            result.StockQuantity.Should().Be(command.StockQuantity);
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCallAddAndCommit()
        {
            // Arrange
            var command = _fixture.Build<CreateProductCommand>()
                .With(x => x.Name, "Notebook Pro")
                .With(x => x.Price, 1000m)
                .With(x => x.StockQuantity, 10)
                .Create();

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldPublishDomainEvent()
        {
            // Arrange
            var command = _fixture.Build<CreateProductCommand>()
                .With(x => x.Name, "Notebook Pro")
                .With(x => x.Price, 1000m)
                .With(x => x.StockQuantity, 10)
                .Create();

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _eventPublisherMock.Verify(
                e => e.PublishAsync(It.IsAny<ProductCreatedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithDuplicateName_ShouldThrowDuplicateProductNameException()
        {
            // Arrange
            var command = _fixture.Build<CreateProductCommand>()
                .With(x => x.Name, "Notebook Pro")
                .With(x => x.Price, 1000m)
                .With(x => x.StockQuantity, 10)
                .Create();

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DuplicateProductNameException>();
        }

        [Fact]
        public async Task Handle_WithDuplicateName_ShouldNotCallAddOrCommit()
        {
            // Arrange
            var command = _fixture.Build<CreateProductCommand>()
                .With(x => x.Name, "Notebook Pro")
                .With(x => x.Price, 1000m)
                .With(x => x.StockQuantity, 10)
                .Create();

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<DuplicateProductNameException>();

            // Assert
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                u => u.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithNegativePrice_ShouldThrowDomainException()
        {
            // Arrange
            var command = _fixture.Build<CreateProductCommand>()
                .With(x => x.Name, "Notebook Pro")
                .With(x => x.Price, -1m)
                .With(x => x.StockQuantity, 10)
                .Create();

            _repositoryMock
                .Setup(r => r.ExistsByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
