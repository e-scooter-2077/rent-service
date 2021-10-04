FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EScooter.RentService.Web/EScooter.RentService.Web.csproj", "EScooter.RentService.Web/"]
COPY ["EScooter.RentService.Infrastructure/EScooter.RentService.Infrastructure.csproj", "EScooter.RentService.Infrastructure/"]
COPY ["EScooter.RentService.Application/EScooter.RentService.Application.csproj", "EScooter.RentService.Application/"]
COPY ["EScooter.RentService.Domain/EScooter.RentService.Domain.csproj", "EScooter.RentService.Domain/"]
RUN dotnet restore "EScooter.RentService.Web/EScooter.RentService.Web.csproj"
COPY . .
WORKDIR "/src/EScooter.RentService.Web"
RUN dotnet build "EScooter.RentService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EScooter.RentService.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EScooter.RentService.Web.dll"]