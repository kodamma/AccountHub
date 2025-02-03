FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /app
EXPOSE 8080

COPY AccountHub.sln ./

COPY ./src/AccountHub.API/AccountHub.API.csproj ./src/AccountHub.API/
COPY ./src/AccountHub.Domain/AccountHub.Domain.csproj ./src/AccountHub.Domain/
COPY ./src/AccountHub.Application/AccountHub.Application.csproj ./src/AccountHub.Application/
COPY ./src/AccountHub.Persistent/AccountHub.Persistent.csproj ./src/AccountHub.Persistent/
COPY ./src/Kodamma.Common.Base.dll ./src/

RUN dotnet restore

COPY ./src/ ./src/

RUN dotnet publish ./src/AccountHub.API/AccountHub.API.csproj -c $BUILD_CONFIGURATION -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "AccountHub.API.dll"]
