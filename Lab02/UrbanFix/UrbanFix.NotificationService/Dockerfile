FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["UrbanFix.NotificationService/UrbanFix.NotificationService.csproj", "UrbanFix.NotificationService/"]
COPY ["UrbanFix.Common/UrbanFix.Common.csproj", "UrbanFix.Common/"]

RUN dotnet restore "UrbanFix.NotificationService/UrbanFix.NotificationService.csproj"

COPY . .

WORKDIR "/src/UrbanFix.NotificationService"
RUN dotnet publish "UrbanFix.NotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrbanFix.NotificationService.dll"]