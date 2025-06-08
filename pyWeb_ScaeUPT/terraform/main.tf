terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  # Configuración del backend para almacenar el estado de Terraform
  # Esto se configurará dinámicamente en el workflow de GitHub Actions
}

provider "azurerm" {
  features {}
  # Estas credenciales se pasarán desde las variables de entorno o secretos de GitHub
  subscription_id = var.subscription_id
  tenant_id       = var.tenant_id
  client_id       = var.client_id
  client_secret   = var.client_secret
}

# Grupo de recursos
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

# Plan de servicio de App Service
resource "azurerm_service_plan" "app_plan" {
  name                = "${var.app_name}-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = var.app_service_sku
  tags                = var.tags
}

# App Service para la aplicación web
resource "azurerm_linux_web_app" "app" {
  name                = var.app_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.app_plan.id
  https_only          = true

  site_config {
    always_on        = true
    ftps_state       = "Disabled"
    health_check_path = "/api/Home"
    
    application_stack {
      docker_image     = "${var.acr_name}.azurecr.io/${var.docker_image_name}"
      docker_image_tag = var.docker_image_tag
    }
  }

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_REGISTRY_SERVER_URL"          = "https://${var.acr_name}.azurecr.io"
    "DOCKER_REGISTRY_SERVER_USERNAME"     = var.acr_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = var.acr_password
    "MYSQL_CONNECTION_STRING"             = var.mysql_connection_string
    "GOOGLE_CLIENT_ID"                    = var.google_client_id
    "GOOGLE_CLIENT_SECRET"                = var.google_client_secret
    "JWT_SECRET_KEY"                      = var.jwt_secret_key
  }

  tags = var.tags
}

# Azure Container Registry (ACR) para almacenar la imagen Docker
resource "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true
  tags                = var.tags
}