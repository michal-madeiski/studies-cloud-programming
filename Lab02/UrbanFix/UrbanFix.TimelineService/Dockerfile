FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["UrbanFix.TimelineService/UrbanFix.TimelineService.csproj", "UrbanFix.TimelineService/"]
COPY ["UrbanFix.Common/UrbanFix.Common.csproj", "UrbanFix.Common/"]

RUN dotnet restore "UrbanFix.TimelineService/UrbanFix.TimelineService.csproj"

COPY . .

WORKDIR "/src/UrbanFix.TimelineService"
RUN dotnet publish "UrbanFix.TimelineService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrbanFix.TimelineService.dll"]