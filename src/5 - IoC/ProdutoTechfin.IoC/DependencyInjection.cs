using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductsApi.CrossCutting.Logging;

namespace ProdutoTechfin.IoC
{
    public static class DependencyInjection
    {
        public static IHostBuilder AddLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.AddSerilogLogging();
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplication();
            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // MediatR, FluentValidation, behaviors — vem aqui quando chegar na Application
            return services;
        }

    }
}
