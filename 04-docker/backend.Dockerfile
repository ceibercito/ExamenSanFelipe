# Dockerfile genérico para cualquier microservicio .NET 8.
# Recibe el nombre del proyecto vía ARG (Auth.API, Compras.API, etc.).
ARG PROJECT

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJECT
WORKDIR /src

# Copiamos todo el backend (solución + proyectos)
COPY 02-backend-dotnet/ ./

# Restauramos y publicamos el microservicio indicado
RUN dotnet restore "${PROJECT}/${PROJECT}.csproj"
RUN dotnet publish "${PROJECT}/${PROJECT}.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG PROJECT
WORKDIR /app
COPY --from=build /app/publish .

# El nombre del dll a ejecutar se inyecta como variable de entorno
ENV DLL_NAME=${PROJECT}.dll
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# entrypoint resuelve el dll en tiempo de ejecución
ENTRYPOINT ["sh", "-c", "dotnet ${DLL_NAME}"]
