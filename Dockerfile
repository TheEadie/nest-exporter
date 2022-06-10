#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.300@sha256:58a93345aa1069d69f4e32eded710dcf5cd563171226fbce69e65b1e98b5e5a5 AS build
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

COPY ./ ./
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
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.5-alpine3.15@sha256:cecc8d413aea2c5c76fdc92ae8a11492e623b4b8b8090ec942db77550504fce1
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]