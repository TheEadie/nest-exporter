default: docker

docker:
	docker build -t theeadie/nest-exporter:dev .

start:
	docker-compose up -d --build

stop:
	docker-compose down