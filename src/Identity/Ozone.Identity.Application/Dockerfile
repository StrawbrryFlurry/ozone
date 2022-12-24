FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Identity/Ozone.Identity.Application/Ozone.Identity.Application.csproj", "Identity/Ozone.Identity.Application/"]
COPY ["Identity/Ozone.Identity.Api/Ozone.Identity.Api.csproj", "Identity/Ozone.Identity.Api/"]
COPY ["Identity/Ozone.Identity.Core/Ozone.Identity.Core.csproj", "Identity/Ozone.Identity.Core/"]
COPY ["Identity/Ozone.Identity.Domain/Ozone.Identity.Domain.csproj", "Identity/Ozone.Identity.Domain/"]
COPY ["Identity/Ozone.Identity.Infrastructure/Ozone.Identity.Infrastructure.csproj", "Identity/Ozone.Identity.Infrastructure/"]
COPY ["Identity/Ozone.Identity.Persistence/Ozone.Identity.Persistence.csproj", "Identity/Ozone.Identity.Persistence/"]

COPY ["Common/*.csproj", "Common/"]
RUN dotnet restore "Identity/Ozone.Identity.Application/Ozone.Identity.Application.csproj"
COPY . .
WORKDIR "/src/Identity/Ozone.Identity.Application"
RUN dotnet build "Ozone.Identity.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ozone.Identity.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozone.Identity.Application.dll"]
