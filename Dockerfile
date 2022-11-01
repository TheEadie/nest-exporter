#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.402@sha256:ce977e0ce71ce4aeecde3917f3abf0dcffcf952e9ca138704b63e1a838b4700c AS build
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
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.10-alpine3.16@sha256:0220063e51b940f8a74605103d083a034c6a81b8b2cd272fc9a9e1c2cc616311
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
