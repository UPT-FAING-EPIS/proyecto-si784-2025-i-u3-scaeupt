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
  sku_name            = "B2"
  
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
  sku_name            = "B2"
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "main" {
  name                = "scae-upt-logs"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Application Insights para ASP.NET App
resource "azurerm_application_insights" "main" {
  name                = "scae-upt-app-insights"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Application Insights para Python Service
resource "azurerm_application_insights" "python" {
  name                = "scae-upt-python-insights"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Action Group para alertas
resource "azurerm_monitor_action_group" "main" {
  name                = "scae-upt-alerts"
  resource_group_name = azurerm_resource_group.main.name
  short_name          = "scaeupt"
  
  email_receiver {
    name          = "admin"
    email_address = var.admin_email
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Alerta de CPU alta para ASP.NET App Service Plan
resource "azurerm_monitor_metric_alert" "cpu_alert_main" {
  name                = "scae-upt-high-cpu-main"
  resource_group_name = azurerm_resource_group.main.name
  scopes              = [azurerm_service_plan.main.id]
  description         = "Alerta cuando el CPU supera el 80% en el plan principal"
  
  criteria {
    metric_namespace = "Microsoft.Web/serverfarms"
    metric_name      = "CpuPercentage"
    aggregation      = "Average"
    operator         = "GreaterThan"
    threshold        = 80
  }
  
  action {
    action_group_id = azurerm_monitor_action_group.main.id
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Alerta de CPU alta para Python App Service Plan
resource "azurerm_monitor_metric_alert" "cpu_alert_python" {
  name                = "scae-upt-high-cpu-python"
  resource_group_name = azurerm_resource_group.main.name
  scopes              = [azurerm_service_plan.python.id]
  description         = "Alerta cuando el CPU supera el 80% en el plan de Python"
  
  criteria {
    metric_namespace = "Microsoft.Web/serverfarms"
    metric_name      = "CpuPercentage"
    aggregation      = "Average"
    operator         = "GreaterThan"
    threshold        = 80
  }
  
  action {
    action_group_id = azurerm_monitor_action_group.main.id
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Alerta de errores HTTP 5xx para ASP.NET Web App
resource "azurerm_monitor_metric_alert" "http_5xx_alert_main" {
  name                = "scae-upt-http-5xx-main"
  resource_group_name = azurerm_resource_group.main.name
  scopes              = [azurerm_linux_web_app.main.id]
  description         = "Alerta cuando hay más de 5 errores HTTP 5xx en 5 minutos en la app principal"
  
  criteria {
    metric_namespace = "Microsoft.Web/sites"
    metric_name      = "Http5xx"
    aggregation      = "Total"
    operator         = "GreaterThan"
    threshold        = 5
  }
  
  action {
    action_group_id = azurerm_monitor_action_group.main.id
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Alerta de errores HTTP 5xx para Python Web App
resource "azurerm_monitor_metric_alert" "http_5xx_alert_python" {
  name                = "scae-upt-http-5xx-python"
  resource_group_name = azurerm_resource_group.main.name
  scopes              = [azurerm_linux_web_app.python_service.id]
  description         = "Alerta cuando hay más de 5 errores HTTP 5xx en 5 minutos en el servicio Python"
  
  criteria {
    metric_namespace = "Microsoft.Web/sites"
    metric_name      = "Http5xx"
    aggregation      = "Total"
    operator         = "GreaterThan"
    threshold        = 5
  }
  
  action {
    action_group_id = azurerm_monitor_action_group.main.id
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Actualizar Linux Web App para ASP.NET con Application Insights
resource "azurerm_linux_web_app" "main" {
  name                = "scae-upt-app"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id
  
  https_only = true
  
  site_config {
    always_on                         = false
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
    
    # Application Insights Configuration
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.main.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION" = "~3"
    "APPINSIGHTS_PROFILERFEATURE_VERSION" = "1.0.0"
    "APPINSIGHTS_SNAPSHOTFEATURE_VERSION" = "1.0.0"
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}

# Actualizar Linux Web App para Python con Application Insights
resource "azurerm_linux_web_app" "python_service" {
  name                = "scae-upt-python-service"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.python.id
  https_only = true

  depends_on = [
    azurerm_service_plan.python
  ]
  
  site_config {
    always_on                         = true
    container_registry_use_managed_identity = false
    
    application_stack {
      docker_image_name        = var.python_docker_image_name
      docker_registry_url      = "https://${azurerm_container_registry.main.login_server}"
      docker_registry_username = azurerm_container_registry.main.admin_username
      docker_registry_password = azurerm_container_registry.main.admin_password
    }
    
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
    
    # Application Insights Configuration
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.python.connection_string
  }
  
  tags = {
    environment = "production"
    project     = "scae-upt"
    managed_by  = "terraform"
  }
}