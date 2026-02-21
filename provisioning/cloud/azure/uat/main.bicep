// Azure Bicep — UAT environment
targetScope = 'resourceGroup'

param location string = resourceGroup().location
param environmentName string = 'uat'
