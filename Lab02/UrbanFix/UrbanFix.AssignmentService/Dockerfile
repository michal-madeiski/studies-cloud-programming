FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["UrbanFix.AssignmentService/UrbanFix.AssignmentService.csproj", "UrbanFix.AssignmentService/"]
COPY ["UrbanFix.Common/UrbanFix.Common.csproj", "UrbanFix.Common/"]

RUN dotnet restore "UrbanFix.AssignmentService/UrbanFix.AssignmentService.csproj"

COPY . .

WORKDIR "/src/UrbanFix.AssignmentService"
RUN dotnet publish "UrbanFix.AssignmentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrbanFix.AssignmentService.dll"]