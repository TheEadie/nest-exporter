#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.405@sha256:6b9c857a60b67f968fd5bae373e414690947b8b58e7951b3228fd712b82938e8 AS build
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
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.4-alpine3.16@sha256:d05253254e0923267bb900607e5adddaa7131343b62d3951c81f8a34f0c1bcc9
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
