FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CreditApp.API/CreditApp.API.csproj", "src/CreditApp.API/"]
COPY ["src/CreditApp.Application/CreditApp.Application.csproj", "src/CreditApp.Application/"]
COPY ["src/CreditApp.Domain/CreditApp.Domain.csproj", "src/CreditApp.Domain/"]
COPY ["src/CreditApp.Infrastructure/CreditApp.Infrastructure.csproj", "src/CreditApp.Infrastructure/"]
RUN dotnet restore "src/CreditApp.API/CreditApp.API.csproj"
COPY . .
WORKDIR "/src/src/CreditApp.API"
RUN dotnet build "CreditApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreditApp.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalar herramientas necesarias
RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

# Instalar herramientas de EF Core y agregar al PATH
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copiar scripts y dar permisos
COPY src/entrypoint.sh /entrypoint.sh
COPY src/wait-for-it.sh /wait-for-it.sh
RUN chmod +x /entrypoint.sh /wait-for-it.sh

ENTRYPOINT ["/entrypoint.sh"] 