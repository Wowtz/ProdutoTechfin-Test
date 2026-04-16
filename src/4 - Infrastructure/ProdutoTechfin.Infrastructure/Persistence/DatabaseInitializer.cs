using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProdutoTechfin.Domain.Entities;

namespace ProdutoTechfin.Infrastructure.Persistence;

public class DatabaseInitializer
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(AppDbContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Database.IsRelational())
        {
            await _context.Database.MigrateAsync(cancellationToken);
        }
        await SeedAsync(cancellationToken);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await _context.Products.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Seeding database...");

        var products = new[]
        {
            Product.Create("Notebook Pro X500", "Notebook de alta performance para desenvolvedores", 4599.99m, 15),
            Product.Create("Mouse Gamer RGB", "Mouse óptico 16000 DPI com iluminação RGB", 249.90m, 42),
            Product.Create("Teclado Mecânico TKL", "Teclado mecânico tenkeyless switches blue", 389.00m, 28)
        };

        await _context.Products.AddRangeAsync(products, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Database seeded successfully with {Count} products.", products.Length);
    }
}