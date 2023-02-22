#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.103@sha256:b71f327441d566f5b07e01d120b15da1089eda98f9665101f6389a3f471b01a5 AS build
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
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.3-alpine3.16@sha256:b80940a662df21ba5b0e9d646bc38f6997f7bb850c70027dd3c09088ae4ac846
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
