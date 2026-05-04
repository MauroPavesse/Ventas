# Usa la versión 8.0 que es la LTS estable (recomendado)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar
# Ajusta los nombres de las carpetas si son distintos
COPY ["Ventas.Api/Ventas.Api.csproj", "Ventas.Api/"]
COPY ["Ventas.Domain/Ventas.Domain.csproj", "Ventas.Domain/"]
COPY ["Ventas.Infrastructure/Ventas.Infrastructure.csproj", "Ventas.Infrastructure/"]
COPY ["Ventas.Application/Ventas.Application.csproj", "Ventas.Application/"]

RUN dotnet restore "Ventas.Api/Ventas.Api.csproj"

# Copiar todo el resto del código
COPY . .
WORKDIR "/src/Ventas.Api"

# Publicar
RUN dotnet publish "Ventas.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ventas.Api.dll"]