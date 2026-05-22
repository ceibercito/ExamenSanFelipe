using Shared.Extensions;
using Ventas.API.Facades;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddSwaggerWithJwt("Ventas.API - Ventas");

builder.Services.AddScoped<IVentaFacade, VentaFacade>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(ServiceCollectionExtensions.CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
