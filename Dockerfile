# =========================
# Etapa 1: Build / Publish
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los proyectos de la solución
COPY ["ClubNet.Api/ClubNet.Api.csproj", "ClubNet.Api/"]
COPY ["ClubNet.Models/ClubNet.Models.csproj", "ClubNet.Models/"]
COPY ["ClubNet.Services/ClubNet.Services.csproj", "ClubNet.Services/"]

# Restauramos dependencias
RUN dotnet restore "ClubNet.Api/ClubNet.Api.csproj"

# Copiamos el resto del código
COPY . .

# Publicamos en modo Release a una carpeta intermedia
RUN dotnet publish "ClubNet.Api/ClubNet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Etapa 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiamos lo publicado desde la etapa de build
COPY --from=build /app/publish .

# Render espera que la app escuche en 0.0.0.0 en algún puerto (usamos 8080)
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Nombre del .dll = nombre del proyecto web (ClubNet.Api)
ENTRYPOINT ["dotnet", "ClubNet.Api.dll"]
