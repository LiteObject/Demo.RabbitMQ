#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5672 

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["Demo.Publisher.API/Demo.Publisher.API.csproj", "Demo.Publisher.API/"]
RUN dotnet restore "Demo.Publisher.API/Demo.Publisher.API.csproj"
COPY . .
WORKDIR "/src/Demo.Publisher.API"
RUN dotnet build "Demo.Publisher.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Publisher.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Publisher.API.dll"]