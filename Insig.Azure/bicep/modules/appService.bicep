param projectName string
param environment string
param location string = 'West Europe'
param frontDoorProfileName string

var appServiceName = '${projectName}-${environment}-api'

resource frontDoorProfile 'Microsoft.Cdn/profiles@2023-05-01' existing = if (frontDoorProfileName != '') {
  name: frontDoorProfileName
}

var ipSecurityRestrictions = frontDoorProfileName == '' ? [] : [
  {
    tag: 'ServiceTag'
    ipAddress: 'AzureFrontDoor.Backend'
    action: 'Allow'
    priority: 100
    headers: {
      'x-azure-fdid': [
        frontDoorProfile.properties.frontDoorId
      ]
    }
    name: 'Allow traffic from Front Door'
  }
]

resource ASPForAppService 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: 'ASP-${appServiceName}'
  location: location
  properties: {
    zoneRedundant: false
  }
  sku: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 0
  }
}

resource ApplicationInsightsForAppService 'Microsoft.Insights/components@2020-02-02' = {
  name: appServiceName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: appServiceName
  kind: 'app'
  location: location
  properties: {
    publicNetworkAccess: 'Enabled'
    serverFarmId: ASPForAppService.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: ApplicationInsightsForAppService.properties.ConnectionString
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: ApplicationInsightsForAppService.properties.InstrumentationKey
        }
      ]
      metadata: [
        {
          name: 'CURRENT_STACK'
          value: 'dotnet'
        }
      ]
      netFrameworkVersion: 'v8.0'
      ipSecurityRestrictions: ipSecurityRestrictions
    }
  }
}

output appServiceName string = appService.name
