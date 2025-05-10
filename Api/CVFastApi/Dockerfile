FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CVFastApi/CVFastApi.csproj", "CVFastApi/"]
RUN dotnet restore "CVFastApi/CVFastApi.csproj"
COPY . .
WORKDIR "/src/CVFastApi"
RUN dotnet build "CVFastApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CVFastApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CVFastApi.dll"]
