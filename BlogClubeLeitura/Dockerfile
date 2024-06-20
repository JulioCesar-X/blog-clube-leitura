# Imagem SDK para desenvolvimento
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /app

# Copiar o conteúdo do diretório atual para o diretório /app no container
COPY ./BlogClubeLeitura/ .

COPY BlogClubeLeitura/entrypoint.sh /entrypoint.sh
# Dar permissões de execução ao script de entrypoint
RUN chmod +x /entrypoint.sh

# Instalar dependências necessárias, incluindo netcat para espera de DB e EF Core CLI
RUN apt-get update && apt-get install -y netcat-openbsd && \
    dotnet tool install --global dotnet-ef

# Configurar PATH para incluir ferramentas globais do .NET
ENV PATH="${PATH}:/root/.dotnet/tools"
# Converter quebras de linha para LF e remover BOM
RUN sed -i '1 s/^\xEF\xBB\xBF//' /entrypoint.sh && \
    sed -i 's/\r$//' /entrypoint.sh


# Definir o entrypoint para o script
ENTRYPOINT ["/entrypoint.sh"]