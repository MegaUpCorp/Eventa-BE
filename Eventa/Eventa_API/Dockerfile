FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Eventa_API/Eventa_API.csproj", "Eventa_API/"]
COPY ["Eventa_Services/Eventa_Services.csproj", "Eventa_Services/"]
COPY ["Eventa_Repositories/Eventa_Repositories.csproj", "Eventa_Repositories/"]
COPY ["Eventa_DAOs/Eventa_DAOs.csproj", "Eventa_DAOs/"]
COPY ["Eventa_BusinessObject/Eventa_BusinessObject.csproj", "Eventa_BusinessObject/"]
RUN dotnet restore "Eventa_API/Eventa_API.csproj"
COPY . .
WORKDIR "/src/Eventa_API"
RUN dotnet build "Eventa_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Eventa_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eventa_API.dll"]
