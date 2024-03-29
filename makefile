IMAGE_NAME := theeadie/nest-exporter
PLATFORMS := linux/amd64,linux/arm64
NEXT_VERSION := 2.0
TAG_PREFIX := 

GITHUB_REPO := theeadie/nest-exporter
GITHUB_AUTH_TOKEN := empty

VERSION := $(shell ./build/version.sh $(NEXT_VERSION))
VERSION_MAJOR := $(word 1,$(subst ., ,$(VERSION)))
VERSION_MINOR := $(word 2,$(subst ., ,$(VERSION)))
VERSION_PATCH := $(word 2,$(subst ., ,$(VERSION)))

.PHONY: default build test

## Build
default: publish-local

publish-local:
	@docker buildx build . \
		-t $(IMAGE_NAME):$(VERSION)-dev \
		--build-arg VERSION=$(VERSION) \
		--load

build-local:
	@docker buildx build . \
		-t $(IMAGE_NAME):$(VERSION)-build \
		--build-arg VERSION=$(VERSION) \
		--target build \
		--load \
		--cache-from=type=gha,scope=build-local \
		--cache-from=type=gha,scope=build \
		--cache-to=type=gha,mode=max,scope=build-local

build:
	@docker buildx build . \
		-t $(IMAGE_NAME):$(VERSION)-build \
		--build-arg VERSION=$(VERSION) \
		--target build \
		--platform $(PLATFORMS) \
		--cache-from=type=gha,scope=build \
		--cache-to=type=gha,mode=max,scope=build

test: build-local
	@docker run --rm -i $(IMAGE_NAME):$(VERSION)-build dotnet test

publish:
	@docker buildx build . \
		-t $(IMAGE_NAME):$(VERSION) \
		--build-arg VERSION=$(VERSION) \
		--platform $(PLATFORMS) \
		--cache-from=type=gha,scope=publish \
		--cache-from=type=gha,scope=build \
		--cache-to=type=gha,mode=max,scope=publish

start:
	@docker-compose build --build-arg VERSION=$(VERSION)
	@docker-compose up --build -d

stop:
	@docker-compose down

version:
	@echo $(VERSION)

lint: | lint-dockerfile lint-sh

lint-dockerfile:
	@docker run --rm -i hadolint/hadolint:v2.12.0 < Dockerfile

lint-sh:
	@docker run --rm -i -v "$(PWD):/mnt" koalaman/shellcheck:v0.9.0 **/*.sh

## Release
release: | docker-push github-release

docker-push:
	@docker buildx build . \
		-t $(IMAGE_NAME):latest \
		-t $(IMAGE_NAME):$(VERSION) \
		-t $(IMAGE_NAME):$(VERSION_MAJOR) \
		-t $(IMAGE_NAME):$(VERSION_MAJOR).$(VERSION_MINOR) \
		--build-arg VERSION=$(VERSION) \
		--platform $(PLATFORMS) \
		--push \
		--cache-from=type=gha,scope=publish \
		--cache-to=type=gha,mode=max,scope=publish

github-release:
	@./build/github-release.sh $(VERSION) $(TAG_PREFIX)$(VERSION) $(GITHUB_AUTH_TOKEN) $(GITHUB_REPO)