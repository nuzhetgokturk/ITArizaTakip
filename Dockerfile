
# 1. Aşama: Uygulamayı derle
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["ITArizaTakip.csproj", "./"]

RUN dotnet restore "ITArizaTakip.csproj"

COPY . .

RUN dotnet publish "ITArizaTakip.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Aşama: Uygulamayı çalıştır
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ITArizaTakip.dll"]
