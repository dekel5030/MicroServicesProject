# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# העתק את קובץ הפרויקט ובצע restore
COPY *.csproj ./
RUN dotnet restore

# העתק את שאר הקבצים ובצע publish
COPY . ./
RUN dotnet publish -c Release -o out

# שלב הריצה
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "PlatformService.dll"]
