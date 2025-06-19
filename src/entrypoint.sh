#!/bin/bash
set -e

# Espera a que SQL Server esté listo
/wait-for-it.sh db 1433 -t 60 -- echo "SQL Server is up"

# Ejecuta las migraciones
export ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Development}

echo "Ejecutando migraciones de Entity Framework..."
if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  # En desarrollo, el código está montado en /src
  cd /src
  /root/.dotnet/tools/dotnet-ef database update --project CreditApp.Infrastructure --startup-project CreditApp.API --verbose
else
  # En producción, usar las rutas del contenedor
  /root/.dotnet/tools/dotnet-ef database update --project /src/CreditApp.Infrastructure --startup-project /src/CreditApp.API
fi

echo "Migraciones completadas. Iniciando API..."

# Arranca la API según el entorno
if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  cd /src/CreditApp.API
  dotnet watch run --urls=http://+:80
else
  dotnet CreditApp.API.dll
fi 