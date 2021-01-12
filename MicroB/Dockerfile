FROM mcr.microsoft.com/dotnet/sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers

COPY ./MicroB ./MicroB
COPY ./MicroCore ./MicroCore
RUN dotnet restore ./MicroB/MicroB.csproj

RUN dotnet publish -c Release -o out  ./MicroB/MicroB.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MicroB.dll"]