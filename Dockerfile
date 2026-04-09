FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FoodTrackerServer/FoodTrackerServer.csproj", "FoodTrackerServer/"]
RUN dotnet restore "./FoodTrackerServer/FoodTrackerServer.csproj"

COPY . .
WORKDIR "/src/FoodTrackerServer"
RUN dotnet build "FoodTrackerServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodTrackerServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FoodTrackerServer.dll"]