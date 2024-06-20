# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY ["BlogClubeLeitura/BlogClubeLeitura.csproj", "BlogClubeLeitura/"]
RUN dotnet restore "BlogClubeLeitura/BlogClubeLeitura.csproj"

# Copy the remaining project files and build the application
COPY BlogClubeLeitura/. BlogClubeLeitura/
WORKDIR /app/BlogClubeLeitura
RUN dotnet build "BlogClubeLeitura.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "BlogClubeLeitura.csproj" -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/publish .

# Install netcat for waiting for database
RUN apt-get update && apt-get install -y netcat-openbsd

# Expose the port
EXPOSE 80
EXPOSE 443

# Define the entrypoint
CMD /bin/bash -c 'while ! nc -z "$PGHOST" "$PGPORT"; do echo "Waiting for the database..."; sleep 1; done; \
  if [ "$SEED_DATABASE" = "true" ]; then dotnet ef database update; fi; dotnet BlogClubeLeitura.dll'