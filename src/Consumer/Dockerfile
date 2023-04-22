FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app
COPY . .
RUN dotnet restore
WORKDIR "/app/src/Consumer"

RUN dotnet build "./Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Consumer.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]