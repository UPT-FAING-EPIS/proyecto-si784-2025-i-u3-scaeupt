# Outputs
output "app_url" {
  value = "https://${azurerm_linux_web_app.main.default_hostname}"
  description = "URL de la aplicación ASP.NET principal"
}

output "python_service_url" {
  value = "https://${azurerm_linux_web_app.python_service.default_hostname}"
  description = "URL del servicio Python de verificación facial"
}

output "acr_login_server" {
  value = azurerm_container_registry.main.login_server
}

output "acr_username" {
  value = azurerm_container_registry.main.admin_username
}

output "acr_password" {
  value = azurerm_container_registry.main.admin_password
  sensitive = true
}

output "resource_group_name" {
  value = azurerm_resource_group.main.name
}

output "application_insights_connection_string" {
  value = azurerm_application_insights.main.connection_string
  description = "Connection string de Application Insights para ASP.NET"
  sensitive = true
}

output "python_application_insights_connection_string" {
  value = azurerm_application_insights.python.connection_string
  description = "Connection string de Application Insights para Python"
  sensitive = true
}

output "application_insights_instrumentation_key" {
  value = azurerm_application_insights.main.instrumentation_key
  description = "Clave de instrumentación de Application Insights para ASP.NET"
  sensitive = true
}

output "log_analytics_workspace_id" {
  value = azurerm_log_analytics_workspace.main.workspace_id
  description = "ID del workspace de Log Analytics"
}

output "application_insights_app_id" {
  value = azurerm_application_insights.main.app_id
  description = "App ID de Application Insights"
}