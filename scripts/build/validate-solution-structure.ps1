# Validate that all projects exist and are referenced in the solution
$csproj = Get-ChildItem -Path (Join-Path $PSScriptRoot '..\..\src') -Recurse -Filter '*.csproj'
Write-Host "Found $($csproj.Count) projects."
