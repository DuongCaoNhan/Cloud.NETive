#Requires -Version 7.0
<#
.SYNOPSIS
    Scan all projects for vulnerable NuGet packages without applying changes.

.DESCRIPTION
    Thin wrapper around fix-vulnerable-packages.ps1 -ScanOnly.
    All parameters are forwarded to the main script.

.EXAMPLE
    .\security-scan.ps1
    Scan using default Moderate+ severity threshold.

.EXAMPLE
    .\security-scan.ps1 -MinSeverity High -IncludeTransitive
    Report only High/Critical including transitive dependencies.
#>
param(
    [string] $Solution          = "",
    [string] $Project           = "",
    [switch] $IncludeTransitive,
    [ValidateSet("Low","Moderate","High","Critical")]
    [string] $MinSeverity       = "Moderate"
)

& "$PSScriptRoot\fix-vulnerable-packages.ps1" `
    -ScanOnly `
    -Solution          $Solution `
    -Project           $Project `
    -IncludeTransitive:$IncludeTransitive `
    -MinSeverity       $MinSeverity

exit $LASTEXITCODE
