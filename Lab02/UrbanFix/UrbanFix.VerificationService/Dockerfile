FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["UrbanFix.VerificationService/UrbanFix.VerificationService.csproj", "UrbanFix.VerificationService/"]
COPY ["UrbanFix.Common/UrbanFix.Common.csproj", "UrbanFix.Common/"]

RUN dotnet restore "UrbanFix.VerificationService/UrbanFix.VerificationService.csproj"

COPY . .

WORKDIR "/src/UrbanFix.VerificationService"
RUN dotnet publish "UrbanFix.VerificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrbanFix.VerificationService.dll"]