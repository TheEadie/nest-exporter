#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.401@sha256:2fdad630a59fbdecbfcc6aaf3ccae96eea0e8d24cd4607669fde0de107a94c53 AS build
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
        -p:Version=${VERSION} \
        -p:TreatWarningsAsErrors=true \
        -p:NoWarn=IL2104

#### Runtime ####
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.9-alpine3.16@sha256:a4cc5aa9352d4a0fc2583f912bc593fd008027aedbd54f71ea27ce081812ea80
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
