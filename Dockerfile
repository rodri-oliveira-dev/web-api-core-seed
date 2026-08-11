FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY global.json ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY WebApiCoreSeed.slnx ./
COPY src/Directory.Packages.props src/
COPY src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj src/WebApiCoreSeed.Api/
COPY src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/
COPY src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/
COPY src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/
RUN dotnet restore src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj

FROM restore AS build
COPY . .
RUN dotnet build src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-restore

FROM build AS publish
RUN dotnet publish src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --output /app/publish --no-restore /p:UseAppHost=false

FROM build AS migrations
RUN dotnet tool install dotnet-ef --tool-path /tools --version 10.0.10
ENV PATH="/tools:${PATH}"
RUN chmod +x scripts/docker/apply-migrations.sh
ENTRYPOINT ["/bin/sh", "/src/scripts/docker/apply-migrations.sh"]

FROM runtime AS final
COPY --from=publish /app/publish .
USER app
ENTRYPOINT ["dotnet", "WebApiCoreSeed.Api.dll"]
