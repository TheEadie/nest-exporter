default: docker

docker:
	docker build -t theeadie/nest-exporter:dev .

run:
	docker-compose up -d

stop:
	docker-compose down

run-windows:
	docker-compose -f docker-compose.yaml -f docker-compose.windows.yaml up -d

stop-windows:
	docker-compose -f docker-compose.yaml -f docker-compose.windows.yaml down
