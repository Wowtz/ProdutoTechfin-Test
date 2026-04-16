using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Infrastructure.Persistence;
using ProdutoTechfin.Infrastructure.Repositories;

namespace ProdutoTechfin.UnitTests.Infrastructure.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region AddAsync

    [Fact]
    public async Task AddAsync_WithValidProduct_ShouldPersistProduct()
    {
        // Arrange
        var product = Product.Create("Notebook Pro", "desc", 1000m, 10);

        // Act
        await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        var persisted = await _context.Products.FindAsync(product.Id);
        persisted.Should().NotBeNull();
        persisted!.Name.Value.Should().Be("Notebook Pro");
        persisted.Price.Amount.Should().Be(1000m);
        persisted.StockQuantity.Value.Should().Be(10);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new[]
        {
            Product.Create("Notebook Pro", "desc", 1000m, 10),
            Product.Create("Mouse Gamer", "desc", 250m, 20),
            Product.Create("Teclado TKL", "desc", 350m, 15)
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProductsOrderedByName()
    {
        // Arrange
        await _context.Products.AddRangeAsync(
            Product.Create("Teclado TKL", "desc", 350m, 15),
            Product.Create("Notebook Pro", "desc", 1000m, 10),
            Product.Create("Mouse Gamer", "desc", 250m, 20)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var names = result.Select(p => p.Name.Value).ToList();

        // Assert
        names.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProducts_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var product = Product.Create("Notebook Pro", "desc", 1000m, 10);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Value.Should().Be("Notebook Pro");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductNotFound_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByIdsAsync

    [Fact]
    public async Task GetByIdsAsync_WithValidIds_ShouldReturnMatchingProducts()
    {
        // Arrange
        var products = new[]
        {
            Product.Create("Notebook Pro", "desc", 1000m, 10),
            Product.Create("Mouse Gamer", "desc", 250m, 20),
            Product.Create("Teclado TKL", "desc", 350m, 15)
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var ids = products.Take(2).Select(p => p.Id).ToList();

        // Act
        var result = await _repository.GetByIdsAsync(ids);

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.Id).Should().BeEquivalentTo(ids);
    }

    [Fact]
    public async Task GetByIdsAsync_WithNonExistentIds_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetByIdsAsync(new[] { Guid.NewGuid(), Guid.NewGuid() });

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region ExistsByNameAsync

    [Fact]
    public async Task ExistsByNameAsync_WhenNameExists_ShouldReturnTrue()
    {
        // Arrange
        var product = Product.Create("Notebook Pro", "desc", 1000m, 10);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByNameAsync("Notebook Pro");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_WhenNameNotExists_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsByNameAsync("Produto Inexistente");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByNameAsync_WhenExcludingOwnId_ShouldReturnFalse()
    {
        // Arrange
        var product = Product.Create("Notebook Pro", "desc", 1000m, 10);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act — mesmo nome mas excluindo o próprio produto (cenário de update)
        var result = await _repository.ExistsByNameAsync("Notebook Pro", excludeId: product.Id);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_WithValidProduct_ShouldPersistChanges()
    {
        // Arrange
        var product = Product.Create("Notebook", "desc", 1000m, 10);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        product.Update("Notebook Pro", "nova desc", 2000m, 20);
        _repository.Update(product);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Products.FindAsync(product.Id);
        updated!.Name.Value.Should().Be("Notebook Pro");
        updated.Price.Amount.Should().Be(2000m);
        updated.StockQuantity.Value.Should().Be(20);
        updated.UpdatedAt.Should().NotBeNull();
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_WithValidProduct_ShouldRemoveFromDatabase()
    {
        // Arrange
        var product = Product.Create("Notebook", "desc", 1000m, 10);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        _repository.Delete(product);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.Products.FindAsync(product.Id);
        deleted.Should().BeNull();
    }

    #endregion

    #region DeleteRange

    [Fact]
    public async Task DeleteRange_WithValidProducts_ShouldRemoveAllFromDatabase()
    {
        // Arrange
        var products = new[]
        {
            Product.Create("Notebook", "desc", 1000m, 10),
            Product.Create("Mouse", "desc", 250m, 20)
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        _repository.DeleteRange(products);
        await _context.SaveChangesAsync();

        // Assert
        var remaining = await _context.Products.ToListAsync();
        remaining.Should().BeEmpty();
    }

    #endregion
}