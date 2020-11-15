param hostingPlanName string
param appName string
param location string = resourceGroup().location
param skuName string {
  default: 'F1'
  allowed: [
    'F1'    
    'S1'
    'S2'    
    'P1'
    'P2'    
  ]
}
param skuCapacity int {
  default: 1
  minValue: 1
  maxValue: 3
}

resource plan 'Microsoft.Web/serverfarms@2020-06-01' ={
 name: hostingPlanName
 location: location
  sku: {
    name: skuName
    capacity: skuCapacity
  }
}

resource webapp 'Microsoft.Web/sites@2020-06-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: plan.id
    siteConfig: {
      remoteDebuggingEnabled:false
    }
  }
}