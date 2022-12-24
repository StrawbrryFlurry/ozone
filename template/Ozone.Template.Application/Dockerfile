FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["template/Ozone.Template.Application/Ozone.Template.Application.csproj", "Ozone.Template.Application/"]
RUN dotnet restore "template/Ozone.Template.Application/Ozone.Template.Application.csproj"
COPY . .
WORKDIR "/src/Ozone.Template.Application"
RUN dotnet build "Ozone.Template.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ozone.Template.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozone.Template.Application.dll"]
