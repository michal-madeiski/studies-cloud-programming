FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["UrbanFix.ReportService/UrbanFix.ReportService.csproj", "UrbanFix.ReportService/"]
COPY ["UrbanFix.Common/UrbanFix.Common.csproj", "UrbanFix.Common/"]

RUN dotnet restore "UrbanFix.ReportService/UrbanFix.ReportService.csproj"

COPY . .

WORKDIR "/src/UrbanFix.ReportService"
RUN dotnet publish "UrbanFix.ReportService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrbanFix.ReportService.dll"]