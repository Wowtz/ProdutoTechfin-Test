using ProdutoTechfin.Api.Extensions;
using ProdutoTechfin.Api.Middleware;
using ProdutoTechfin.Infrastructure.Persistence;
using ProdutoTechfin.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLogging();

builder.Services.AddCorsExtensions();

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCustomSwagger();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();