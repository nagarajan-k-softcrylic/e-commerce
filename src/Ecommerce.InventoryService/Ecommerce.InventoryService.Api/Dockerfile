FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5102

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Ecommerce.InventoryService/Ecommerce.InventoryService.Api/Ecommerce.InventoryService.Api.csproj", "src/Ecommerce.InventoryService/Ecommerce.InventoryService.Api/"]
COPY ["src/Ecommerce.InventoryService/Ecommerce.InventoryService.Application/Ecommerce.InventoryService.Application.csproj", "src/Ecommerce.InventoryService/Ecommerce.InventoryService.Application/"]
COPY ["src/Ecommerce.InventoryService/Ecommerce.InventoryService.Domain/Ecommerce.InventoryService.Domain.csproj", "src/Ecommerce.InventoryService/Ecommerce.InventoryService.Domain/"]
COPY ["src/Ecommerce.InventoryService/Ecommerce.InventoryService.Persistence/Ecommerce.InventoryService.Persistence.csproj", "src/Ecommerce.InventoryService/Ecommerce.InventoryService.Persistence/"]
COPY ["src/Ecommerce.InventoryService/Ecommerce.InventoryService.Infrastructure/Ecommerce.InventoryService.Infrastructure.csproj", "src/Ecommerce.InventoryService/Ecommerce.InventoryService.Infrastructure/"]
COPY ["src/Ecommerce.Shared/Ecommerce.Shared.csproj", "src/Ecommerce.Shared/"]
RUN dotnet restore "src/Ecommerce.InventoryService/Ecommerce.InventoryService.Api/Ecommerce.InventoryService.Api.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.InventoryService/Ecommerce.InventoryService.Api"
RUN dotnet build "Ecommerce.InventoryService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce.InventoryService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.InventoryService.Api.dll"]

