job "payments-database" {
  type = "service"

  datacenters = ["dc1"]

  group "payments" {
    count = 1

    network {
      mode = "bridge"
      port "tcp" {
        to = "5432"
      }
    }

    service {
      name = "payments-db"
      port = "5432"

      connect {
        sidecar_service {
          proxy {
            # expose the metrics endpont 
            config {
              envoy_prometheus_bind_addr = "0.0.0.0:9102"
            }
          }
        }
      }
    }

    task "payments" {
      driver = "docker"

      config {
        image = "postgres:14.2"
        ports = ["http"]
      }

      env {
        POSTGRES_USER     = "payments"
        POSTGRES_PASSWORD = "password"
        POSTGRES_DB       = "payments"
      }

      resources {
        cpu    = 500 # MHz
        memory = 128 # MB
      }
    }
  }
}
