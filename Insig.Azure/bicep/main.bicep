targetScope = 'subscription'

param projectName string

param location string = 'West Europe'

@allowed([ 'uat', 'prod' ])
param environment string = 'uat'

param sqlServerLogin string = 'softintadmin'

@secure()
param sqlServerPassword string

param addFrontDoor bool = false

param wafEnableLimitToPoland bool = false

resource resrcGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: '${projectName}${environment}'
  location: location
}

module frontDoorProfile './modules/frontDoorProfile.bicep' = if (addFrontDoor == true) {
  scope: resourceGroup(resrcGroup.name)
  name: 'FrontDoorProfile'
  params: {
    environment: environment
    projectName: projectName
  }
}

module webApp './modules/webApp.bicep' = {
  scope: resourceGroup(resrcGroup.name)
  name: 'StaticWebApp'
  params: {
    environment: environment
    projectName: projectName
    location: location
  }
}

module appService './modules/appService.bicep' = {
  scope: resourceGroup(resrcGroup.name)
  name: 'AppService'
  params: {
    environment: environment
    projectName: projectName
    location: location
    frontDoorProfileName: addFrontDoor == true ? frontDoorProfile.outputs.frontDoorProfileName : ''
  }
}

module storage './modules/storage.bicep' = {
  scope: resourceGroup(resrcGroup.name)
  name: 'Storage'
  params: {
    environment: environment
    projectName: projectName
    location: location
  }
}

module sql './modules/sql.bicep' = {
  scope: resourceGroup(resrcGroup.name)
  name: 'Sql'
  params: {
    environment: environment
    projectName: projectName
    location: location
    sqlServerLogin: sqlServerLogin
    sqlServerPassword: sqlServerPassword
  }
}

module frontDoor './modules/frontdoor.bicep' = if (addFrontDoor == true) {
  scope: resourceGroup(resrcGroup.name)
  name: 'FrontDoor'
  params: {
    environment: environment
    projectName: projectName
    webAppName: webApp.outputs.webAppName
    appServiceName: appService.outputs.appServiceName
    storageName: storage.outputs.storageName
    frontDoorProfileName: addFrontDoor == true ? frontDoorProfile.outputs.frontDoorProfileName : ''
    wafEnableLimitToPoland: wafEnableLimitToPoland
  }
}
