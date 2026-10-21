terraform {
  required_version = ">= 1.5"

  required_providers {
    neon = {
      source  = "kislerdm/neon"
      version = "0.18.0" # fijada: el repo del provider esta archivado
    }
    render = {
      source  = "render-oss/render"
      version = "1.9.1"
    }
  }
}

provider "neon" {}   # lee NEON_API_KEY
provider "render" {} # lee RENDER_API_KEY y RENDER_OWNER_ID

# --- Base de datos de PREPROD: no existe hasta que esto se aplica ---

resource "neon_project" "preprod" {
  name   = "appgastos-preprod"
  org_id = var.neon_org_id

  # El tope del plan gratuito es 6 horas; el provider pide 1 dia por defecto y el primer apply falla
  history_retention_seconds = 21600
}

resource "neon_role" "preprod" {
  project_id = neon_project.preprod.id
  branch_id  = neon_project.preprod.default_branch_id
  name       = "app_preprod_owner"
}

resource "neon_database" "preprod" {
  project_id = neon_project.preprod.id
  branch_id  = neon_project.preprod.default_branch_id
  name       = "app_preprod"
  owner_name = neon_role.preprod.name
}

# --- Servicios de PREPROD: corren la imagen del TP7 ---

locals {
  # La cadena de conexion se ARMA con lo que devolvio Neon: nadie la copia y pega
  conn = "Host=${neon_project.preprod.database_host};Database=${neon_database.preprod.name};Username=${neon_role.preprod.name};Password=${neon_role.preprod.password};SSL Mode=Require"
}

resource "render_web_service" "api" {
  name   = "appgastos-api-preprod"
  plan   = "free"
  region = var.region

  runtime_source = {
    image = {
      image_url = var.image_repo_api # SIN la etiqueta
      tag       = var.image_tag
    }
  }

  env_vars = {
    ConnectionStrings__Default = { value = local.conn }
    PORT                       = { value = "8080" }
  }
}

resource "render_web_service" "front" {
  name   = "appgastos-front-preprod"
  plan   = "free"
  region = var.region

  runtime_source = {
    image = {
      image_url = var.image_repo_front
      tag       = var.image_tag
    }
  }

  env_vars = {
    BACKEND_URL  = { value = render_web_service.api.url } # sale de Terraform, no de un copy-paste
    DNS_RESOLVER = { value = "8.8.8.8" }
    PORT         = { value = "80" }
  }
}
