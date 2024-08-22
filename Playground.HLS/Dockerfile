# Stage 1: Base image with FFmpeg installed
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y ffmpeg

# Stage 2: Build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Playground.HLS.csproj", "./"]
RUN dotnet restore "Playground.HLS.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Playground.HLS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Playground.HLS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Playground.HLS.dll"]
