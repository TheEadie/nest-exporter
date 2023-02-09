#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.102@sha256:70cc3456a1ccedb70b945e1e62cf865660ec6a65797b2bf9ef2eb201a29a8dd7 AS build
WORKDIR /app

ARG TARGETPLATFORM
RUN case "${TARGETPLATFORM}" in \
         "linux/amd64")  RID=x64  ;; \
         "linux/arm64")  RID=arm64  ;; \
    esac \
    && echo "${RID}" > RID
SHELL ["/bin/bash", "-o", "pipefail", "-c"]

COPY *.sln ./
COPY src/nest-exporter/nest-exporter.csproj ./src/nest-exporter/
COPY src/nest-exporter.tests/nest-exporter.tests.csproj ./src/nest-exporter.tests/
RUN dotnet restore --runtime alpine-"$(cat RID)"

COPY .editorconfig ./src/
COPY src/ ./src/
ARG VERSION=0.0.1
RUN dotnet publish \
        "src/nest-exporter/nest-exporter.csproj" \
        -c Release \
        --runtime alpine-"$(cat RID)" \
        --self-contained \
        -o out \
        --no-restore \
        -p:AssemblyVersion=${VERSION} \
        -p:Version=${VERSION}

#### Runtime ####
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.2-alpine3.16@sha256:fe6f63ef87eb8a61ad91ef04ae13094ee5bd734ff3bbfb8d43b0d6b1c5f20b3c
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
