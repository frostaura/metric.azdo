# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Metric.Azdo.Api/Metric.Azdo.Api.csproj", "Metric.Azdo.Api/"]
RUN dotnet restore "Metric.Azdo.Api/Metric.Azdo.Api.csproj"
COPY . .
WORKDIR "/src/Metric.Azdo.Api"
RUN dotnet build "Metric.Azdo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Metric.Azdo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Metric.Azdo.Api.dll"]