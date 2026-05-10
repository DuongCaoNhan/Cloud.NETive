#Requires -Version 7.0
<#
.SYNOPSIS
    Solution-wide package health check: outdated packages + vulnerability scan.

.DESCRIPTION
    Runs two checks:
      1. dotnet list package --outdated  (informational, no changes made)
      2. fix-vulnerable-packages.ps1 -ScanOnly  (security scan)

.PARAMETER SolutionRoot
    Path to the repo root. Defaults to three levels above $PSScriptRoot.

.PARAMETER SkipVulnerabilityScan
    Skip the vulnerability scan step.

.PARAMETER SkipOutdatedCheck
    Skip the outdated-package check step.
#>
param(
    [string] $SolutionRoot         = "",
    [switch] $SkipVulnerabilityScan,
    [switch] $SkipOutdatedCheck
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-Root {
    if ($SolutionRoot -ne "") { return $SolutionRoot }
    $up = $PSScriptRoot
    1..3 | ForEach-Object { $up = Split-Path $up -Parent }
    return $up
}

$root = Get-Root
$line = "=" * 72

Write-Host ""
Write-Host $line -ForegroundColor Cyan
Write-Host "  Package Health Check  --  CloudNative Solution" -ForegroundColor Cyan
Write-Host $line -ForegroundColor Cyan

# ── 1. Outdated packages ────────────────────────────────────────────────────
if (-not $SkipOutdatedCheck) {
    Write-Host ""
    Write-Host "  Outdated Packages" -ForegroundColor Yellow
    Write-Host "  -----------------" -ForegroundColor DarkGray
    Write-Host ""

    $slnFile = Get-ChildItem -Path "$root\*" -Include "*.slnx","*.sln" `
                             -ErrorAction SilentlyContinue | Select-Object -First 1

    if ($slnFile) {
        & dotnet list $slnFile.FullName package --outdated
    } else {
        Get-ChildItem -Path $root -Recurse -Filter "*.csproj" |
            Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
            ForEach-Object {
                Write-Host "  $($_.Name)" -ForegroundColor DarkGray
                & dotnet list $_.FullName package --outdated
            }
    }
}

# ── 2. Vulnerability scan ───────────────────────────────────────────────────
if (-not $SkipVulnerabilityScan) {
    Write-Host ""
    Write-Host "  Vulnerability Scan" -ForegroundColor Yellow
    Write-Host "  ------------------" -ForegroundColor DarkGray

    & "$PSScriptRoot\fix-vulnerable-packages.ps1" -ScanOnly -IncludeTransitive
}

Write-Host ""
Write-Host $line -ForegroundColor Cyan
Write-Host "  Health check complete." -ForegroundColor Cyan
Write-Host $line -ForegroundColor Cyan
Write-Host ""
