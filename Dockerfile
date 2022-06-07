#### Build ####
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.300@sha256:58a93345aa1069d69f4e32eded710dcf5cd563171226fbce69e65b1e98b5e5a5 AS build
WORKDIR /app

ARG TARGETPLATFORM
RUN case ${TARGETPLATFORM} in \
         "linux/amd64")  RID=x64  ;; \
         "linux/arm64")  RID=arm64  ;; \
    esac \
    && echo ${RID} > RID

COPY src/*.csproj ./
RUN dotnet restore --runtime alpine-"$(cat RID)"

COPY src/ .editorconfig ./
ARG VERSION=0.0.1
RUN dotnet build \
        -c Release \
        --runtime alpine-"$(cat RID)" \
        --self-contained \
        -p:AssemblyVersion=${VERSION} \
        -p:Version=${VERSION} \
        -p:TreatWarningsAsErrors=true \
        --no-restore

#### Publish ####
FROM --platform=$BUILDPLATFORM build AS publish
WORKDIR /app

RUN dotnet publish \
        -c Release \
        --runtime alpine-"$(cat RID)" \
        --self-contained \
        -p:PublishTrimmed=true \
        -o out \
        --no-build

#### Runtime ####
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.5-alpine3.15@sha256:cecc8d413aea2c5c76fdc92ae8a11492e623b4b8b8090ec942db77550504fce1
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["./nest-exporter"]
