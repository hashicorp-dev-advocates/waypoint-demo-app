project = "hashicraft"

runner {
  enabled = true
}

app "payments-deployment" {
  build {
    use "docker" {}

    registry {
      use "docker" {
        image = "10.10.0.10/hashicraft/payments"
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

