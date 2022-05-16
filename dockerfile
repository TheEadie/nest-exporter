FROM mcr.microsoft.com/dotnet/sdk:6.0.300-alpine3.15@sha256:9d5f437adddaacf4c980b29b69c5176572ec7dd029a67db7e54a4b243524441d AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/ ./
RUN dotnet publish -c Release -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0.5-alpine3.15@sha256:c44e49a486c7643dd0ea7c63f3f9bf0d6318d6a86666348047145654327b0b46
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
