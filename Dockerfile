#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./MekanikApi/MekanikApi.Api.csproj", "MekanikApi/"]
COPY ["./MekanikApi.Application/MekanikApi.Application.csproj", "MekanikApi.Application/"]
COPY ["./MekanikApi.Domain/MekanikApi.Domain.csproj", "MekanikApi.Domain/"]
COPY ["./MekanikApi.Infrastructure/MekanikApi.Infrastructure.csproj", "MekanikApi.Infrastructure/"]
RUN dotnet restore "./MekanikApi/MekanikApi.Api.csproj"
COPY . .
WORKDIR "/src/MekanikApi"
RUN dotnet build "MekanikApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MekanikApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MekanikApi.Api.dll"]