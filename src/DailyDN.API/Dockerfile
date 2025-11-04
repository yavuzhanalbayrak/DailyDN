# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Tüm src klasörünü kopyala
COPY . .

# Restore işlemi API projesi üzerinden
WORKDIR /src/DailyDN.API
RUN dotnet restore "DailyDN.API.csproj"

# Build ve publish
RUN dotnet publish "DailyDN.API.csproj" -c Release -o /app/publish

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "DailyDN.API.dll"]
