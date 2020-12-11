param location string = resourceGroup().location
param namespaceName string
var serviceBusNamespaceName  = concat(namespaceName, '-packtmapbook')
var hubNamespaceName = concat(namespaceName, '-',uniqueString(resourceGroup().id))
var storageAccountName = concat(namespaceName,uniqueString(resourceGroup().id))
resource namespace 'Microsoft.EventHub/namespaces@2017-04-01' = {
  name: '${hubNamespaceName}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
    capacity: 1
  }
  properties: {}
}

resource eventHub 'Microsoft.EventHub/namespaces/eventhubs@2017-04-01' = {
  name: '${namespace.name}/dapreh'
  properties: {}
}

resource consumerGroup 'Microsoft.EventHub/namespaces/eventhubs/consumergroups@2017-04-01' = {
  name: '${eventHub.name}/shipping'
  properties: {}
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2018-01-01-preview' = {
  name: '${serviceBusNamespaceName}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {}
}

resource serviceBusQueue 'Microsoft.ServiceBus/namespaces/topics@2017-04-01' = {
  name: '${serviceBusNamespace.name}/dapr'  
}

resource sa 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource container 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${sa.name}/default/packt'
}

