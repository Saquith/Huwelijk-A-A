# Multi-stage Dockerfile that builds the Blazor client and backend, then produces a runtime image.
# Build context should be the repository root that contains WeddingSite/ and WeddingBackend/ directories.

# Stage 1: build/publish Blazor client
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client
WORKDIR /src

COPY WeddingSite/ WeddingSite/
RUN dotnet restore WeddingSite/WeddingSite.csproj
RUN dotnet publish WeddingSite/WeddingSite.csproj -c Release -o /app/publish/client

# Stage 2: publish backend and include client in wwwroot
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

COPY WeddingBackend/ WeddingBackend/
COPY --from=build-client /app/publish/client/ WeddingBackend/wwwroot/

RUN dotnet restore WeddingBackend/WeddingBackend.csproj
RUN dotnet publish WeddingBackend/WeddingBackend.csproj -c Release -o /app/publish/backend

# Stage 3: runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# create logs directory
RUN mkdir -p /app/logs && chmod 0777 /app/logs

# Copy published backend (includes wwwroot with client)
COPY --from=build-backend /app/publish/backend/ ./

# Copy entrypoint script and make executable
COPY WeddingBackend/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["./entrypoint.sh"]