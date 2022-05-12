FROM mcr.microsoft.com/dotnet/sdk:6.0.300-alpine3.15@sha256:9d5f437adddaacf4c980b29b69c5176572ec7dd029a67db7e54a4b243524441d AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/ ./
RUN dotnet publish --runtime alpine-x64 -c Release --self-contained true -p:PublishTrimmed=true -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.4-alpine3.15@sha256:5f654b6a88a4752ea1751b2dc4229c57db7bac8436fb201a64667d32e23fdbc9
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
