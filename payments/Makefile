run:
	dotnet run .

build_docker:
	docker build -t 10.10.0.10/payments:latest .

run_local_db:
	docker run -d \
		--name paymentsdb \
		-e POSTGRES_PASSWORD=password \
		-e POSTGRES_USER=payments \
		-e POSTGRES_DB=payments \
		-p 5432:5432 \
		postgres