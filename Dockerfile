# Imagen base para aplicaciones ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Imagen base para la compilación del proyecto
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiamos todas las capas del proyecto
COPY ["CapaPresentationAdmin/CapaPresentationAdmin.csproj", "CapaPresentationAdmin/"]
COPY ["CapaDatos/CapaDatos.csproj", "CapaDatos/"]
COPY ["CapaEntidad/CapaEntidad.csproj", "CapaEntidad/"]
COPY ["CapaNegocio/CapaNegocio.csproj", "CapaNegocio/"]

# Restauramos las dependencias de cada capa
RUN dotnet restore "CapaPresentationAdmin/CapaPresentationAdmin.csproj"

# Copiamos el código fuente de todas las capas
COPY . .

# Establecemos el directorio de trabajo en la capa de presentación
WORKDIR "/src/CapaPresentationAdmin"

# Compilamos la solución
RUN dotnet build "CapaPresentationAdmin.csproj" -c Release -o /app/build

# Publicamos la solución
FROM build AS publish
RUN dotnet publish "CapaPresentationAdmin.csproj" -c Release -o /app/publish

# Imagen final de ejecución
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configuración para Python
WORKDIR /api
COPY CapaPresentationAdmin/API/ /api
RUN apt-get update && apt-get install -y git
RUN pip install --upgrade pip
RUN pip install -r /api/requirements.txt

# Configuramos la ejecución del proyecto ASP.NET Core
WORKDIR /app
ENTRYPOINT ["dotnet", "CapaPresentationAdmin.dll"]

# Ejecutamos el script Python
CMD ["python3", "/api/main.py"]