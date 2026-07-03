FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BackendAkademija.api/BackendAkademija.api.csproj", "BackendAkademija.api/"]
COPY ["BackendAkademija.Application/BackendAkademija.Application.csproj", "BackendAkademija.Application/"]
COPY ["BackendAkademija.Domain/BackendAkademija.Domain.csproj", "BackendAkademija.Domain/"]
COPY ["BackendAkademija.Infrastructure/BackendAkademija.Infrastructure.csproj", "BackendAkademija.Infrastructure/"]

RUN dotnet restore "BackendAkademija.api/BackendAkademija.api.csproj"

COPY . .

RUN dotnet build "BackendAkademija.api/BackendAkademija.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackendAkademija.api/BackendAkademija.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendAkademija.api.dll"]