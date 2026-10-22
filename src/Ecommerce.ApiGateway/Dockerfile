FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Ecommerce.ApiGateway/Ecommerce.ApiGateway.csproj", "src/Ecommerce.ApiGateway/"]
COPY ["src/Ecommerce.Shared/Ecommerce.Shared.csproj", "src/Ecommerce.Shared/"]
RUN dotnet restore "src/Ecommerce.ApiGateway/Ecommerce.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.ApiGateway"
RUN dotnet build "Ecommerce.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.ApiGateway.dll"]

