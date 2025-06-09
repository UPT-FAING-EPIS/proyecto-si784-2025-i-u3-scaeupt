terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
  
  backend "azurerm" {
    # La configuración se pasa via backend-config en el workflow
  }
}

provider "azurerm" {
  features {}
  
  subscription_id = var.subscription_id
  tenant_id       = var.tenant_id
  client_id       = var.client_id
  client_secret   = var.client_secret
}


# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "scae-upt-rg"
  location = "East US"
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Container Registry
resource "azurerm_container_registry" "main" {
  name                = "scaeuptacr"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"
  admin_enabled       = true
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "scae-upt-app-plan"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "F1"
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Linux Web App
resource "azurerm_linux_web_app" "main" {
  name                = "scae-upt-app"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id
  
  https_only = true
  
  site_config {
    always_on                         = false  # F1 plan no soporta always_on
    container_registry_use_managed_identity = false
    
    application_stack {
      docker_image_name        = "pyweb-scaeupt"
      docker_registry_url      = "https://${azurerm_container_registry.main.login_server}"
      docker_registry_username = azurerm_container_registry.main.admin_username
      docker_registry_password = azurerm_container_registry.main.admin_password
    }
    
    health_check_path = "/api/Home"
  }
  
  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_REGISTRY_SERVER_URL"          = "https://${azurerm_container_registry.main.login_server}"
    "DOCKER_REGISTRY_SERVER_USERNAME"     = azurerm_container_registry.main.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = azurerm_container_registry.main.admin_password
    "MYSQL_CONNECTION_STRING"             = var.mysql_connection_string
    "GOOGLE_CLIENT_ID"                    = var.google_client_id
    "GOOGLE_CLIENT_SECRET"                = var.google_client_secret
    "JWT_SECRET_KEY"                      = var.jwt_secret_key
    "DOCKER_IMAGE_TAG"                    = var.docker_image_tag
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}