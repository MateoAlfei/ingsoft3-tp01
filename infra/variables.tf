variable "image_repo_api" {
  default = "ghcr.io/mateoalfei/ingsoft3-tp01-backend" # SIN la etiqueta
}

variable "image_repo_front" {
  default = "ghcr.io/mateoalfei/ingsoft3-tp01-frontend" # SIN la etiqueta
}

variable "image_tag" {
  default = "sha-a9c04f54fba5fd24cd88e1e8e1f7591d669495d2" # imagen del release v7.0.0
}

variable "region" {
  default = "ohio"
}

variable "neon_org_id" {
  default = "org-old-dew-27910766" # id de la organizacion de Neon (no es un secreto)
}
