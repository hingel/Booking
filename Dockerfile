#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-arm64v8 AS base
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
FROM mcr.microsoft.com/dotnet/sdk:8.0-noble-arm64v8 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Booking.Host/Booking.Host.csproj", "Booking.Host/"]
COPY ["Booking.Business/Booking.Business.csproj", "Booking.Business/"]
COPY ["Booking.DataAccess/Booking.DataAccess.csproj", "Booking.DataAccess/"]
RUN dotnet restore "./Booking.Host/Booking.Host.csproj"
COPY . .
WORKDIR "/src/Booking.Host"
RUN dotnet build "./Booking.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Booking.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Host.dll"]