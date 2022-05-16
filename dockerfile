FROM mcr.microsoft.com/dotnet/sdk:6.0.300-alpine3.15@sha256:9d5f437adddaacf4c980b29b69c5176572ec7dd029a67db7e54a4b243524441d AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/ ./
RUN dotnet publish -c Release -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0.5-alpine3.15@sha256:2e7c6d7e48c949505ae11a3f4c4f0d146910637b5dfd679343574f24783b9bbe
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
