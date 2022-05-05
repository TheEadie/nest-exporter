FROM mcr.microsoft.com/dotnet/sdk:6.0.202-alpine3.15 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/ ./
RUN dotnet publish --runtime alpine-x64 -c Release --self-contained true -p:PublishTrimmed=true -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.4-alpine3.15
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]