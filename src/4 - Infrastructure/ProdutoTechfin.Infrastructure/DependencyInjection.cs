using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Repositories;
using ProdutoTechfin.Infrastructure.Messaging;
using ProdutoTechfin.Infrastructure.Messaging.Consumers;
using ProdutoTechfin.Infrastructure.Persistence;
using ProdutoTechfin.Infrastructure.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace ProdutoTechfin.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureExtensions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddRepositories();
            services.AddMessaging(configuration);

            return services;
        }

        private static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var useInMemory = configuration.GetValue<bool>("DatabaseSettings:UseInMemoryDatabase");

            if (useInMemory)
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("ProdutoTechfinDb"));
            }
            else
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("PostgreSQL"),
                        npgsql => npgsql.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null)));
            }

            services.AddScoped<DatabaseInitializer>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddMessaging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var useInMemory = configuration.GetValue<bool>("DatabaseSettings:UseInMemoryDatabase");

            services.AddMassTransit(bus =>
            {
                bus.AddConsumer<ProductCreatedEventConsumer>();
                bus.AddConsumer<ProductUpdatedEventConsumer>();
                bus.AddConsumer<ProductDeletedEventConsumer>();

                if (useInMemory)
                {
                    bus.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                }
                else
                {
                    bus.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(configuration["RabbitMQ:Host"], "/", host =>
                        {
                            host.Username(configuration["RabbitMQ:Username"]!);
                            host.Password(configuration["RabbitMQ:Password"]!);
                        });

                        cfg.PrefetchCount = 16;

                        cfg.ConfigureEndpoints(context);
                    });
                }
            });

            services.AddScoped<IProductEventPublisher, ProductEventPublisher>();

            return services;
        }
    }
}
