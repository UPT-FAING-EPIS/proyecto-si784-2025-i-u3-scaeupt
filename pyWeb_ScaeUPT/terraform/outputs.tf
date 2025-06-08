output "app_url" {
  description = "URL de la aplicación web"
  value       = "https://${azurerm_linux_web_app.app.default_hostname}"
}

output "acr_login_server" {
  description = "Servidor de inicio de sesión del ACR"
  value       = azurerm_container_registry.acr.login_server
}