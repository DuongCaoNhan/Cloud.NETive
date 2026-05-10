#Requires -Version 7.0
<#
.SYNOPSIS
    Fix vulnerable OpenTelemetry packages across the solution.

.DESCRIPTION
    Delegates to fix-vulnerable-packages.ps1, which queries the NuGet API for
    the latest stable versions and updates all PackageReference entries in place.

    The solution's primary OpenTelemetry versions live in:
        src/BuildingBlocks/CloudNative.ServiceDefaults/CloudNative.ServiceDefaults.csproj

.EXAMPLE
    .\update-opentelemetry.ps1
    Auto-fix any OpenTelemetry NU1902/NU1903 warnings.

.EXAMPLE
    .\update-opentelemetry.ps1 -ScanOnly
    Report without making changes.
#>
param(
    [switch] $ScanOnly,
    [switch] $WhatIf
)

$extra = @()
if ($ScanOnly) { $extra += "-ScanOnly" }
if ($WhatIf)   { $extra += "-WhatIf"   }

Write-Host "  Targeting OpenTelemetry packages via fix-vulnerable-packages.ps1 ..." -ForegroundColor Cyan

& "$PSScriptRoot\fix-vulnerable-packages.ps1" @extra

exit $LASTEXITCODE
