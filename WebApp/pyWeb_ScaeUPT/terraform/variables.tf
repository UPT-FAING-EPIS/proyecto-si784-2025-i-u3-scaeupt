variable "resource_group_name" {
  description = "Nombre del grupo de recursos"
  type        = string
  default     = "scae-upt-rg"
}

variable "location" {
  description = "Ubicación de los recursos de Azure"
  type        = string
  default     = "eastus" #westus
}

variable "app_name" {
  description = "Nombre de la aplicación web"
  type        = string
  default     = "scae-upt-app"
}

variable "app_service_sku" {
  description = "SKU del plan de App Service"
  type        = string
  default     = "F1"  # Cambiar de F1 a B1 (Basic)
}

variable "python_app_service_sku" {
  description = "SKU del plan de App Service para Python"
  type        = string
  default     = "B2"  # Plan más robusto para el microservicio Python
}

variable "acr_name" {
  description = "Nombre del Azure Container Registry"
  type        = string
  default     = "scaeuptacr"
}

variable "docker_image_name" {
  description = "Nombre de la imagen Docker"
  type        = string
  default     = "pyweb-scaeupt"
}

variable "docker_image_tag" {
  description = "Tag de la imagen Docker"
  type        = string
  default     = "latest"
}

# variables para servicio Python
variable "python_docker_image_name" {
  description = "Nombre de la imagen Docker para el servicio Python"
  type        = string
  default     = "python-verificador"
}

variable "python_docker_image_tag" {
  description = "Tag de la imagen Docker para el servicio Python"
  type        = string
  default     = "latest"
}

variable "mysql_connection_string" {
  description = "Cadena de conexión a MySQL"
  type        = string
  sensitive   = true
}

variable "google_client_id" {
  description = "ID de cliente de Google"
  type        = string
  sensitive   = true
}

variable "google_client_secret" {
  description = "Secreto de cliente de Google"
  type        = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "Clave secreta para JWT"
  type        = string
  sensitive   = true
}

variable "subscription_id" {
  description = "ID de la suscripción de Azure"
  type        = string
  sensitive   = true
}

variable "tenant_id" {
  description = "ID del tenant de Azure"
  type        = string
  sensitive   = true
}

variable "client_id" {
  description = "ID del cliente de Azure (Service Principal)"
  type        = string
  sensitive   = true
}

variable "client_secret" {
  description = "Secreto del cliente de Azure (Service Principal)"
  type        = string
  sensitive   = true
}

variable "tags" {
  description = "Etiquetas para los recursos"
  type        = map(string)
  default     = {
    environment = "production"
    project     = "scae-upt"
  }
}

variable "admin_email" {
  description = "Email del administrador para recibir alertas"
  type        = string
}