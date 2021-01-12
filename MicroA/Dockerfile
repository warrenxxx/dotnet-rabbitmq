FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers

COPY ./MicroA ./MicroA
COPY ./MicroCore ./MicroCore
RUN dotnet restore ./MicroA/MicroA.csproj

RUN dotnet publish -c Release -o out  ./MicroA/MicroA.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MicroA.dll"]