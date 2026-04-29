FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore CMS.sln
RUN dotnet publish CmsBackend/CmsBackend.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "CmsBackend.dll"]