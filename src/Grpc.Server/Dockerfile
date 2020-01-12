FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Grpc.Server/Grpc.Server.csproj", "src/Grpc.Server/"]
RUN dotnet restore "src/Grpc.Server/Grpc.Server.csproj"
COPY . .
WORKDIR "/src/src/Grpc.Server"
RUN dotnet build "Grpc.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Grpc.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Grpc.Server.dll"]