# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY Satlink.Api/Satlink.Api.csproj Satlink.Api/
COPY Satlink.Logic/Satink.Logic.csproj Satlink.Logic/
COPY Satlink.Infrastructure/Satlink.Infrastructure.csproj Satlink.Infrastructure/
COPY Satlink.Domain/Satlink.Domain.csproj Satlink.Domain/
RUN dotnet restore Satlink.Api/Satlink.Api.csproj

# Copy everything else and publish
COPY . .
WORKDIR /src/Satlink.Api
RUN dotnet publish Satlink.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Satlink.Api.dll"]
