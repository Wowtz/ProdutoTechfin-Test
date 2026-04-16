using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdutoTechfin.Domain.Entities;
using ProdutoTechfin.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace ProdutoTechfin.Infrastructure.Persistence.Configurations
{
    [ExcludeFromCodeCoverage]
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);

            builder.Property(p => p.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("updated_at");

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired()
                .HasConversion(
                    name => name.Value,
                    value => new ProductName(value));

            builder.HasIndex(p => p.Name)
                .IsUnique();

            builder.OwnsOne(p => p.Price, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("price")
                    .HasColumnType("numeric(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(p => p.StockQuantity)
                .HasColumnName("stock_quantity")
                .IsRequired()
                .HasConversion(
                    stock => stock.Value,
                    value => new StockQuantity(value));

            builder.Ignore(b => b.DomainEvents);
        }
    }
}
