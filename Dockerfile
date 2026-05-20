# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY src/FiapCloudGames.Users.Domain/FiapCloudGames.Users.Domain.csproj ./FiapCloudGames.Users.Domain/
COPY src/FiapCloudGames.Users.Application/FiapCloudGames.Users.Application.csproj ./FiapCloudGames.Users.Application/
COPY src/FiapCloudGames.Users.Infrastructure/FiapCloudGames.Users.Infrastructure.csproj ./FiapCloudGames.Users.Infrastructure/
COPY src/FiapCloudGames.Users.API/FiapCloudGames.Users.API.csproj ./FiapCloudGames.Users.API/

RUN dotnet restore ./FiapCloudGames.Users.API/FiapCloudGames.Users.API.csproj

COPY src/FiapCloudGames.Users.Domain/ ./FiapCloudGames.Users.Domain/
COPY src/FiapCloudGames.Users.Application/ ./FiapCloudGames.Users.Application/
COPY src/FiapCloudGames.Users.Infrastructure/ ./FiapCloudGames.Users.Infrastructure/
COPY src/FiapCloudGames.Users.API/ ./FiapCloudGames.Users.API/

WORKDIR /src/FiapCloudGames.Users.API
RUN dotnet build FiapCloudGames.Users.API.csproj -c Release --no-restore

# Stage 2: Publicação
FROM build AS publish
RUN dotnet publish FiapCloudGames.Users.API.csproj -c Release --no-build -o /app/publish

# Stage 3: Runtime (Final)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

RUN addgroup -g 1000 -S appgroup && \
    adduser -u 1000 -S appuser -G appgroup


RUN apk add --no-cache \
    icu-libs \
    tzdata \
    ca-certificates


ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=America/Sao_Paulo


RUN mkdir -p /app/logs && chown -R appuser:appgroup /app/logs

COPY --from=publish --chown=appuser:appgroup /app/publish .

USER appuser

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FiapCloudGames.Users.API.dll"]
