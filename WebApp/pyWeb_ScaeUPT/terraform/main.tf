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

# App Service Plan para ASP.NET
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

# App Service Plan para Python (microservicio)
resource "azurerm_service_plan" "python" {
  name                = "scae-upt-python-plan"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = var.python_app_service_sku  # Usar B2 o superior
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Linux Web App para ASP.NET (existente)
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

# Linux Web App para Python (nuevo microservicio)
resource "azurerm_linux_web_app" "python_service" {
  name                = "scae-upt-python-service"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.python.id
  
  https_only = true
  
  site_config {
    always_on                         = true  # F1 plan no soporta always_on
    container_registry_use_managed_identity = false
    
    application_stack {
      docker_image_name        = var.python_docker_image_name
      docker_registry_url      = "https://${azurerm_container_registry.main.login_server}"
      docker_registry_username = azurerm_container_registry.main.admin_username
      docker_registry_password = azurerm_container_registry.main.admin_password
    }
    
    # Health check para el servicio Python
    health_check_path = "/verificar"
  }
  
  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_REGISTRY_SERVER_URL"          = "https://${azurerm_container_registry.main.login_server}"
    "DOCKER_REGISTRY_SERVER_USERNAME"     = azurerm_container_registry.main.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = azurerm_container_registry.main.admin_password
    "DOCKER_IMAGE_TAG"                    = var.python_docker_image_tag
    "WEBSITES_PORT"                       = "5000"
    "FLASK_ENV"                          = "production"
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}