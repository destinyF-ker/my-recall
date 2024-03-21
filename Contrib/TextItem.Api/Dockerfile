FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5163

ENV ASPNETCORE_URLS=http://+:5163

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Contrib/TextItem.Api/TextItem.Api.csproj", "Contrib/TextItem.Api/"]
RUN dotnet restore "Contrib/TextItem.Api/TextItem.Api.csproj"
COPY . .
WORKDIR "/src/Contrib/TextItem.Api"
RUN dotnet build "TextItem.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "TextItem.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecAll.Contrib.TextItem.Api.dll"]
