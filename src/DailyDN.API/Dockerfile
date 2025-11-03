# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Tüm projeleri kopyala
COPY ../.. .

# API projesini restore et
WORKDIR /app/src/DailyDN.API
RUN dotnet restore

# Build
RUN dotnet publish -c Release -o /app/out

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Uygulama 8080 portunu dinlesin
EXPOSE 8080

ENTRYPOINT ["dotnet", "DailyDN.API.dll"]
