using Compras.API.Facades;
using Compras.API.Repositories;
using Compras.API.Services;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddSwaggerWithJwt("Compras.API - Productos y Compras");

// Inyección de dependencias (DIP de SOLID)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICompraFacade, CompraFacade>();

// PATRÓN DECORATOR: el controlador recibe el decorador de logging, que envuelve
// al ProductoService real. Así añadimos comportamiento sin tocar el servicio.
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<IProductoService>(sp =>
    new ProductoServiceLoggingDecorator(
        sp.GetRequiredService<ProductoService>(),
        sp.GetRequiredService<ILogger<ProductoServiceLoggingDecorator>>()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(ServiceCollectionExtensions.CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
