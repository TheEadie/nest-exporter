FROM hadolint/hadolint:2.10.0-alpine@sha256:588dc410f08b4dbbf5fa94f9d0c6b4baaf4216815d5adfce9ceb306af61c7cce as lint-dockerfile
WORKDIR /app
COPY Dockerfile .
RUN /bin/hadolint Dockerfile

FROM koalaman/shellcheck-alpine:v0.8.0@sha256:f42fde76d2d14a645a848826e54a4d650150e151d9c81057c898da89a82c8a56 as lint-sh
WORKDIR /app
COPY build/* ./
RUN /bin/shellcheck ./*.sh

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.300-alpine3.15@sha256:9d5f437adddaacf4c980b29b69c5176572ec7dd029a67db7e54a4b243524441d AS build
WORKDIR /app

# Write the dotnet format RID to the filesystem for use later
ARG TARGETPLATFORM
RUN case ${TARGETPLATFORM} in \
         "linux/amd64")  RID=x64  ;; \
         "linux/arm64")  RID=arm64  ;; \
    esac \
    && echo ${RID} > RID

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore --runtime alpine-"$(cat RID)"

# Copy everything else and build
COPY src/ .editorconfig ./
ARG VERSION=0.0.1
RUN dotnet publish \
        -c Release \
        --runtime alpine-"$(cat RID)" \
        --self-contained \
        -p:PublishTrimmed=true \
        -p:AssemblyVersion=${VERSION} \
        -p:Version=${VERSION} \
        -o out \
        --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.5-alpine3.15@sha256:cecc8d413aea2c5c76fdc92ae8a11492e623b4b8b8090ec942db77550504fce1
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
