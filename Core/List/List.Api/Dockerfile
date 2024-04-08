FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5231

ENV ASPNETCORE_URLS=http://+:5231

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Core/List/List.Api/List.Api.csproj", "Core/List/List.Api/"]
RUN dotnet restore "Core/List/List.Api/List.Api.csproj"
COPY . .
WORKDIR "/src/Core/List/List.Api"
RUN dotnet build "List.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "List.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecAll.Core.List.Api.dll"]
