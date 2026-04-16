using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi;
using System.Diagnostics.CodeAnalysis;

namespace ProdutoTechfin.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("x-api-version")
                );
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger(setup =>
        {
            setup.PreSerializeFilters.Add((document, request) =>
                document.Servers = new List<OpenApiServer>
                {
                    new() { Url = $"https://{request.Host.Value}" }
                }
            );
        })
        .UseSwaggerUI(options =>
        {
            var provider = app.ApplicationServices
                .GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var item in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{item.GroupName}/swagger.json",
                    item.GroupName.ToUpperInvariant());
            }

            options.RoutePrefix = "swagger";
        });

        return app;
    }
}