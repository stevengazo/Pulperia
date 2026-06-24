# ── Etapa 1: build/publish ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar primero solo el .csproj para aprovechar la cache de capas
COPY ["Pulperia.csproj", "./"]
RUN dotnet restore "Pulperia.csproj"

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish "Pulperia.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ── Etapa 2: runtime ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# QuestPDF (SkiaSharp) necesita libfontconfig1 para renderizar PDFs en Linux
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Directorio de logs escribible por el usuario no-root
RUN mkdir -p /app/logs && chown -R app:app /app
USER app

# Kestrel escucha en 8080 (TLS lo termina un reverse proxy por delante)
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Pulperia.dll"]
