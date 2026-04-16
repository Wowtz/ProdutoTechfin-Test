using System.Diagnostics.CodeAnalysis;

namespace ProdutoTechfin.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsExtensions(this IServiceCollection services)
        {
            return services.AddCors(options =>
                {
                    options.AddPolicy("FrontendPolicy", policy =>
                    {
                        policy
                            .WithOrigins(
                                "http://localhost:4200",
                                "https://localhost:4200"
                            )
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });
        }
    }
}
