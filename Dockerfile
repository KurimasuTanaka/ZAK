FROM mcr.microsoft.com/dotnet/sdk:9.0-noble AS build
WORKDIR /src

RUN ls ./
COPY . .
RUN dotnet restore

RUN dotnet publish ZAK/ZAK/ZAK.csproj -c Release --no-restore /maxcpucount:1 -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble
WORKDIR /app
COPY --from=build /app/publish ./
COPY kyiv.osm.pbf ./

ENTRYPOINT ["dotnet", "ZAK.dll"]