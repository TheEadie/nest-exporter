FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.300@sha256:58a93345aa1069d69f4e32eded710dcf5cd563171226fbce69e65b1e98b5e5a5 AS build
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
        -p:TreatWarningsAsErrors=true \
        -p:WarningsNotAsErrors=IL2104 \
        -o out \
        --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.5-alpine3.15@sha256:cecc8d413aea2c5c76fdc92ae8a11492e623b4b8b8090ec942db77550504fce1
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./nest-exporter"]
