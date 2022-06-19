project = "hashicraft"

runner {
  enabled = true
}

app "payments-deployment" {
  build {
    use "docker" {}

    registry {
      use "docker" {
        image = "10.5.0.100/hashicraft/payments"
        tag   = "latest"
      }
    }
  }

  deploy {
    use "nomad-jobspec" {
      jobspec = templatefile("${path.app}/deploy/payments.nomad")
    }
  }

  release {}
}

app "payments-db" {
  build {
    use "noop" {}
  }

  deploy {
    use "nomad-jobspec" {
      jobspec = templatefile("${path.app}/deploy/payments-db.nomad")
    }
  }

  release {}
}
