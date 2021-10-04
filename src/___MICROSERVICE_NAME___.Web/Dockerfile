FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["___MICROSERVICE_NAME___.Web/___MICROSERVICE_NAME___.Web.csproj", "___MICROSERVICE_NAME___.Web/"]
COPY ["___MICROSERVICE_NAME___.Infrastructure/___MICROSERVICE_NAME___.Infrastructure.csproj", "___MICROSERVICE_NAME___.Infrastructure/"]
COPY ["___MICROSERVICE_NAME___.Application/___MICROSERVICE_NAME___.Application.csproj", "___MICROSERVICE_NAME___.Application/"]
COPY ["___MICROSERVICE_NAME___.Domain/___MICROSERVICE_NAME___.Domain.csproj", "___MICROSERVICE_NAME___.Domain/"]
RUN dotnet restore "___MICROSERVICE_NAME___.Web/___MICROSERVICE_NAME___.Web.csproj"
COPY . .
WORKDIR "/src/___MICROSERVICE_NAME___.Web"
RUN dotnet build "___MICROSERVICE_NAME___.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "___MICROSERVICE_NAME___.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "___MICROSERVICE_NAME___.Web.dll"]