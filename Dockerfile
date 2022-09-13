#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.401@sha256:a788c58ec0604889912697286ce7d6a28a12ec28d375250a7cd547b619f19b37 AS build
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
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.8-alpine3.15@sha256:f5d816de85b96d03a0eeafbeac62339550bcabb12fa920768eac4417182bee4a
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]