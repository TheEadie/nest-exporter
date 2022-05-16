default: docker

docker:
	docker build -t theeadie/nest-exporter:dev .

build:
	docker buildx build -t theeadie/nest-exporter:build . --platform linux/amd64,linux/arm64

start:
	docker-compose up -d --build

stop:
	docker-compose down