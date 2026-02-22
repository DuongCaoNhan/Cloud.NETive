# Upgrade Aspire packages to 13.x
# Aspire.AppHost.Sdk, Aspire.Hosting.AppHost, Aspire.* components → 13.1.1

param([string]$Version = '13.1.1')

$root = Split-Path $PSScriptRoot -Parent | Split-Path -Parent | Split-Path -Parent
$csproj = Get-ChildItem -Path $root -Recurse -Filter '*.csproj'

foreach ($file in $csproj) {
    $content = Get-Content $file.FullName -Raw
    $updated = $content `
        -replace '(Aspire\.AppHost\.Sdk"\s+Version=")[^"]+', "`${1}$Version" `
        -replace '(Aspire\.Hosting\.AppHost"\s+Version=")[^"]+', "`${1}$Version" `
        -replace '(Aspire\.[^"]+"\s+Version=")[^"]+(?="\s*/>)', "`${1}$Version"
    if ($updated -ne $content) {
        Set-Content $file.FullName $updated -Encoding UTF8
        Write-Host "  Updated: $($file.Name)"
    }
}
Write-Host "Done. All Aspire packages → $Version"
