#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Jarvis.Server/Jarvis.Server.csproj", "Jarvis.Server/"]
COPY ["Jarvis.SstCloud.Core/Jarvis.SstCloud.Core.csproj", "Jarvis.SstCloud.Core/"]
COPY ["Jarvis.SstCloud.Client/Jarvis.SstCloud.Client.csproj", "Jarvis.SstCloud.Client/"]
COPY ["Jarvis.Core/Jarvis.Core.csproj", "Jarvis.Core/"]
RUN dotnet restore "Jarvis.Server/Jarvis.Server.csproj"
COPY . .
WORKDIR "/src/Jarvis.Server"
RUN dotnet build "Jarvis.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Jarvis.Server.csproj" -c Release -o /app/publish

FROM base AS final
ENV CONFIG_FOLDER_PATH="/config"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jarvis.Server.dll"]

#FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-buster-slim
#WORKDIR /app
#COPY . .
#ENTRYPOINT ["dotnet", "/app/Jarvis.Server.dll"]