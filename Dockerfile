#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.100@sha256:39069a126f2e3dd8f3aa72a21688d4de13ba23b051a8c5856be1460f8165c422 AS build
WORKDIR /app

ARG TARGETPLATFORM
RUN case ${TARGETPLATFORM} in \
         "linux/amd64")  RID=x64  ;; \
         "linux/arm64")  RID=arm64  ;; \
    esac \
    && echo ${RID} > RID
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
        -p:PublishTrimmed=true \
        -o out \
        --no-restore \
        -p:AssemblyVersion=${VERSION} \
        -p:Version=${VERSION} 

#### Runtime ####
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.0-alpine3.16@sha256:7eed62ae41f250cd9beed1dd0f28770c26fabae0995e48562dc372dfa1f50ced
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
