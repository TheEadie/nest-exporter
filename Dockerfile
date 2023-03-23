#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.202@sha256:f712881bafadf0e56250ece1da28ba2baedd03fb3dd49a67f209f9d0cf928e81 AS build
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
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.4-alpine3.16@sha256:b4c4c9c042f1d44b7292e22f4f6e3f3a878a9c3bfcdfae83162283a5a71341a5
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
