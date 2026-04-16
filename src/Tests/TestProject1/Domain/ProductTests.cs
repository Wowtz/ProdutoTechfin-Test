using FluentAssertions;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.Events;
using ProdutoTechfin.Domain.Exceptions;

namespace ProdutoTechfin.UnitTests.Domain
{
    public class ProductTests
    {
        #region Create

        [Fact]
        public void Create_WithValidData_ShouldCreateProduct()
        {
            // Arrange
            var name = "Notebook Pro";
            var description = "Notebook de alta performance";
            var price = 4599.99m;
            var stockQuantity = 10;

            // Act
            var product = Product.Create(name, description, price, stockQuantity);

            // Assert
            product.Id.Should().NotBeEmpty();
            product.Name.Value.Should().Be(name);
            product.Description.Should().Be(description);
            product.Price.Amount.Should().Be(price);
            product.Price.Currency.Should().Be("BRL");
            product.StockQuantity.Value.Should().Be(stockQuantity);
            product.IsActive.Should().BeTrue();
            product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            product.UpdatedAt.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidName_ShouldThrowDomainException(string invalidName)
        {
            // Act
            var act = () => Product.Create(invalidName, "desc", 100m, 10);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Create_WithNegativePrice_ShouldThrowDomainException()
        {
            // Act
            var act = () => Product.Create("Notebook", "desc", -1m, 10);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*negative*");
        }

        [Fact]
        public void Create_WithNegativeStock_ShouldThrowDomainException()
        {
            // Act
            var act = () => Product.Create("Notebook", "desc", 100m, -1);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*negative*");
        }

        [Fact]
        public void Create_ShouldRaiseProductCreatedEvent()
        {
            // Act
            var product = Product.Create("Notebook", "desc", 100m, 10);

            // Assert
            product.DomainEvents.Should().ContainSingle();
            product.DomainEvents.First().Should().BeOfType<ProductCreatedEvent>();
        }

        [Fact]
        public void Create_ProductCreatedEvent_ShouldHaveCorrectProductId()
        {
            // Act
            var product = Product.Create("Notebook", "desc", 100m, 10);
            var domainEvent = product.DomainEvents.First() as ProductCreatedEvent;

            // Assert
            domainEvent!.Id.Should().Be(product.Id);
        }

        #endregion

        #region Update

        [Fact]
        public void Update_WithValidData_ShouldUpdateProduct()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 100m, 10);
            var newName = "Notebook Pro";
            var newPrice = 200m;

            // Act
            product.Update(newName, "nova desc", newPrice, 20);

            // Assert
            product.Name.Value.Should().Be(newName);
            product.Price.Amount.Should().Be(newPrice);
            product.StockQuantity.Value.Should().Be(20);
            product.UpdatedAt.Should().NotBeNull();
            product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Update_ShouldRaiseProductUpdatedEvent()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 100m, 10);
            product.ClearDomainEvents();

            // Act
            product.Update("Notebook Pro", "nova desc", 200m, 20);

            // Assert
            product.DomainEvents.Should().ContainSingle();
            product.DomainEvents.First().Should().BeOfType<ProductUpdatedEvent>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Update_WithInvalidName_ShouldThrowDomainException(string invalidName)
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 100m, 10);

            // Act
            var act = () => product.Update(invalidName, "desc", 100m, 10);

            // Assert
            act.Should().Throw<DomainException>();
        }

        #endregion

        #region Deactivate

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 100m, 10);

            // Act
            product.Deactivate();

            // Assert
            product.IsActive.Should().BeFalse();
            product.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Deactivate_ShouldRaiseProductDeletedEvent()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 100m, 10);
            product.ClearDomainEvents();

            // Act
            product.Deactivate();

            // Assert
            product.DomainEvents.Should().ContainSingle();
            product.DomainEvents.First().Should().BeOfType<ProductDeletedEvent>();
        }

        #endregion

        #region ClearDomainEvents

        [Fact]
        public void ClearDomainEvents_ShouldRemoveAllEvents()
        {
            // Arrange
            var product = Product.Create("Notebook", "desc", 100m, 10);
            product.DomainEvents.Should().NotBeEmpty();

            // Act
            product.ClearDomainEvents();

            // Assert
            product.DomainEvents.Should().BeEmpty();
        }

        #endregion
    }
}
