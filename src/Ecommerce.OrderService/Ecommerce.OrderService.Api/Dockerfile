FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5101

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Ecommerce.OrderService/Ecommerce.OrderService.Api/Ecommerce.OrderService.Api.csproj", "src/Ecommerce.OrderService/Ecommerce.OrderService.Api/"]
COPY ["src/Ecommerce.OrderService/Ecommerce.OrderService.Application/Ecommerce.OrderService.Application.csproj", "src/Ecommerce.OrderService/Ecommerce.OrderService.Application/"]
COPY ["src/Ecommerce.OrderService/Ecommerce.OrderService.Domain/Ecommerce.OrderService.Domain.csproj", "src/Ecommerce.OrderService/Ecommerce.OrderService.Domain/"]
COPY ["src/Ecommerce.OrderService/Ecommerce.OrderService.Persistence/Ecommerce.OrderService.Persistence.csproj", "src/Ecommerce.OrderService/Ecommerce.OrderService.Persistence/"]
COPY ["src/Ecommerce.OrderService/Ecommerce.OrderService.Infrastructure/Ecommerce.OrderService.Infrastructure.csproj", "src/Ecommerce.OrderService/Ecommerce.OrderService.Infrastructure/"]
COPY ["src/Ecommerce.Shared/Ecommerce.Shared.csproj", "src/Ecommerce.Shared/"]
RUN dotnet restore "src/Ecommerce.OrderService/Ecommerce.OrderService.Api/Ecommerce.OrderService.Api.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.OrderService/Ecommerce.OrderService.Api"
RUN dotnet build "Ecommerce.OrderService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce.OrderService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.OrderService.Api.dll"]

