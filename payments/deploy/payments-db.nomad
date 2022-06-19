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
            config {
              protocol = "tcp"
            }
          }
        }
      }
    }

    task "payments" {
      driver = "docker"

      config {
        image = "postgres:14.2"
        ports = ["tcp"]
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
