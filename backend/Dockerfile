# Etapa 1: compilar la aplicacion
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar los archivos de proyecto primero (mejor cache de Docker)
COPY ConstruControl.Domain/*.csproj ConstruControl.Domain/
COPY ConstruControl.Application/*.csproj ConstruControl.Application/
COPY ConstruControl.Infrastructure/*.csproj ConstruControl.Infrastructure/
COPY ConstruControl.API/*.csproj ConstruControl.API/
COPY ConstruControl.slnx ./

RUN dotnet restore ConstruControl.API/ConstruControl.API.csproj

# Copiar el resto del codigo y compilar
COPY . .
RUN dotnet publish ConstruControl.API/ConstruControl.API.csproj -c Release -o /app/publish

# Etapa 2: imagen final, mas liviana, solo con el runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render asigna el puerto via la variable de entorno PORT
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ConstruControl.API.dll"]
