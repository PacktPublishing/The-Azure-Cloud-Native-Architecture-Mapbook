provider "azurerm"{
    version="=2.36.0"
    features {}
}
data "azurerm_resource_group" "packt"{
    name="packt"
}
resource "azurerm_app_service_plan" "packt-mapbook-terraform-plan" {
  name = "packt-mapbook-terraform-plan"
  location = data.azurerm_resource_group.packt.location
  resource_group_name = data.azurerm_resource_group.packt.name
  sku {
    tier = "Free"
    size = "F1"
  }  
}   
resource "azurerm_app_service" "packt-mapbook-terraform-app-service" {
  name = "packt-mapbook-terraform-app-service"
  location = data.azurerm_resource_group.packt.location
  resource_group_name = data.azurerm_resource_group.packt.name
  app_service_plan_id = azurerm_app_service_plan.packt-mapbook-terraform-plan.id  
}
