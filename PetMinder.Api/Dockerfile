FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PetMinder.Api/PetMinder.Api.csproj", "PetMinder.Api/"]
COPY ["PetMinder.Shared/PetMinder.Shared.csproj", "PetMinder.Shared/"]
RUN dotnet restore "PetMinder.Api/PetMinder.Api.csproj"
COPY . .
WORKDIR "/src/PetMinder.Api"
RUN dotnet build "PetMinder.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PetMinder.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetMinder.Api.dll"]
