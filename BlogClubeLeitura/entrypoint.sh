#!/bin/bash

# Esperar o banco de dados estar pronto
echo "Aguardando pelo banco de dados..."
while ! nc -z viaduct.proxy.rlwy.net 37314; do
  sleep 1
done
echo "Banco de dados pronto."

# Condicionalmente resetar o banco de dados e migrações
if [ "$SEED_DATABASE" = "true" ]; then
  echo "Resetando o banco de dados..."
  dotnet ef database update 0
  dotnet ef migrations remove
  dotnet ef migrations add InitialCreate --output-dir Data/Migrations
  dotnet ef database update
else
  echo "Aplicando migrações existentes..."
  dotnet ef database update
fi

# Iniciar a aplicação
echo "Iniciando a aplicação..."
exec "$@"