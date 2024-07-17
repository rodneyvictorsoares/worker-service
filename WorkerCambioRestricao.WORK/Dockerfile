#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine3.16 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WorkerCambioRestricao.WORK/WorkerCambioRestricao.WORK.csproj", "WorkerCambioRestricao.WORK/"]
COPY ["WorkerCambioRestricao.APPLICATION/WorkerCambioRestricao.APPLICATION.csproj", "WorkerCambioRestricao.APPLICATION/"]
COPY ["WorkerCambioRestricao.DOMAIN/WorkerCambioRestricao.DOMAIN.csproj", "WorkerCambioRestricao.DOMAIN/"]
COPY ["WorkerCambioRestricao.INFRA/WorkerCambioRestricao.INFRA.csproj", "WorkerCambioRestricao.INFRA/"]
RUN dotnet restore "WorkerCambioRestricao.WORK/WorkerCambioRestricao.WORK.csproj"
COPY . .
WORKDIR "/src/WorkerCambioRestricao.WORK"
RUN dotnet build "WorkerCambioRestricao.WORK.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerCambioRestricao.WORK.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV LogPath="\logs\WORKCAMBIORESTRICAOTRACE.txt"
#ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER 0
#ENTRYPOINT ["dotnet", "WorkerCambioRestricao.WORK.dll"]
ENTRYPOINT ["sh", "-c", "mkdir -p /logs && dotnet WorkerCambioRestricao.WORK.dll"]