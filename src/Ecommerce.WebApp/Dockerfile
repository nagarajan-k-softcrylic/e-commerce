FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Ecommerce.WebApp/Ecommerce.WebApp.csproj", "src/Ecommerce.WebApp/"]
COPY ["src/Ecommerce.Shared/Ecommerce.Shared.csproj", "src/Ecommerce.Shared/"]
RUN dotnet restore "src/Ecommerce.WebApp/Ecommerce.WebApp.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.WebApp"
RUN dotnet publish "Ecommerce.WebApp.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY src/Ecommerce.WebApp/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

