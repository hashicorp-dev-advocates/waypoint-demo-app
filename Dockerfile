FROM alpine:latest

RUN apk update && apk add ca-certificates curl unzip && rm -rf /var/cache/apk/*

RUN wget https://github.com/nicholasjackson/fake-service/releases/download/v0.23.1/fake_service_linux_amd64.zip; \
      unzip fake_service_linux_amd64.zip; \
      mkdir /app; \
      mv ./fake-service /app/; \
      chmod +x /app/fake-service


ENV LISTEN_ADDR="0.0.0.0:3000"

ENTRYPOINT ["/app/fake-service"]
