FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5231

ENV ASPNETCORE_URLS=http://+:5231

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Infrastructure/ServiceStatus/ServiceStatus.csproj", "Infrastructure/ServiceStatus/"]
RUN dotnet restore "Infrastructure/ServiceStatus/ServiceStatus.csproj"
COPY . .
WORKDIR "/src/Infrastructure/ServiceStatus"
RUN dotnet build "ServiceStatus.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ServiceStatus.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecAll.Infrastructure.ServiceStatus.Api.dll"]
