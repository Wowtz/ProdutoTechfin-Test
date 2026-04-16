using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductsApi.CrossCutting.Logging;
using System.Diagnostics.CodeAnalysis;
using ProdutoTechfin.Application;
using ProdutoTechfin.Infrastructure;

namespace ProdutoTechfin.IoC
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IHostBuilder AddLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.AddSerilogLogging();
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddApplicationExtensions();
            return services;
        }

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddInfrastructureExtensions(configuration);
            return services;
        }
    }
}
