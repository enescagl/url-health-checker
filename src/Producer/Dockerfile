FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app
COPY Producer.csproj .
RUN dotnet restore
COPY . .

RUN dotnet build "./Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Producer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Producer.dll"]