IMAGE_NAME = theeadie/nest-exporter
PLATFORMS = linux/amd64,linux/arm64
NEXT_VERSION = 0.1
TAG_PREFIX = nest-exporter/

GITHUB_REPO = theeadie/home-server
GITHUB_AUTH_TOKEN = empty

VERSION = $(shell ./.build/version.sh $(NEXT_VERSION))
VERSION_MAJOR = $(word 1,$(subst ., ,$(VERSION)))
VERSION_MINOR = $(word 2,$(subst ., ,$(VERSION)))
VERSION_PATCH = $(word 2,$(subst ., ,$(VERSION)))

## Build
default: build

build:
	@docker buildx build . \
		-t $(IMAGE_NAME):$(VERSION)-dev \
		--build-arg VERSION=$(VERSION) \
		--load

build-all-platforms:
	@docker buildx build . \
		-t $(IMAGE_NAME):$(VERSION) \
		--build-arg VERSION=$(VERSION) \
		--platform $(PLATFORMS)

start:
	@docker-compose build --build-arg VERSION=$(VERSION)
	@docker-compose up -d

stop:
	@docker-compose down

version:
	@echo $(VERSION)

lint: | lint-dockerfile

lint-dockerfile:
	@docker buildx build . \
		--target lint-dockerfile

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
		--push

github-release:
	@./.build/github-release.sh $(VERSION) $(TAG_PREFIX)$(VERSION) $(GITHUB_AUTH_TOKEN) $(GITHUB_REPO)