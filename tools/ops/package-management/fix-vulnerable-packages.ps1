#Requires -Version 7.0
<#
.SYNOPSIS
    Auto-detects and fixes NuGet package vulnerability warnings (NU1902, NU1903).

.DESCRIPTION
    Scans all .csproj files across the solution for vulnerable NuGet packages,
    queries the NuGet API for the latest patched stable version, and updates
    PackageReference entries in-place.

    Transitive vulnerabilities are reported but cannot be auto-fixed; the tool
    emits a ready-to-paste <PackageReference> override snippet for each one.

.PARAMETER Solution
    Path to the .slnx / .sln file or the solution root directory.
    Defaults to three directories above $PSScriptRoot (repo root).

.PARAMETER Project
    Path to a single .csproj file. Only that project is scanned when provided.

.PARAMETER ScanOnly
    Report vulnerabilities without writing any changes to disk.

.PARAMETER IncludeTransitive
    Also scan transitive (indirect) dependencies.

.PARAMETER MinSeverity
    Minimum severity to process: Low | Moderate | High | Critical (default: Moderate)

.PARAMETER NoRestore
    Skip running 'dotnet restore' after applying fixes.

.PARAMETER CpmFile
    Path to a Central Package Management file (Directory.Packages.props).

.EXAMPLE
    .\fix-vulnerable-packages.ps1
    Scan and auto-fix all Moderate+ direct-dependency vulnerabilities.

.EXAMPLE
    .\fix-vulnerable-packages.ps1 -ScanOnly
    Report only — no files are changed.

.EXAMPLE
    .\fix-vulnerable-packages.ps1 -WhatIf
    Preview every change without writing to disk.

.EXAMPLE
    .\fix-vulnerable-packages.ps1 -MinSeverity High -IncludeTransitive
    Fix only High/Critical; also list transitive vulnerabilities.

.EXAMPLE
    .\fix-vulnerable-packages.ps1 -Project src\BuildingBlocks\CloudNative.ServiceDefaults\CloudNative.ServiceDefaults.csproj
    Scan and fix a single project.
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Solution         = "",
    [string] $Project          = "",
    [switch] $ScanOnly,
    [switch] $IncludeTransitive,
    [ValidateSet("Low","Moderate","High","Critical")]
    [string] $MinSeverity      = "Moderate",
    [switch] $NoRestore,
    [string] $CpmFile          = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$SeverityOrder  = @{ Low = 1; Moderate = 2; High = 3; Critical = 4 }
$SeverityColors = @{ Low = "White"; Moderate = "Yellow"; High = "Red"; Critical = "DarkRed" }
$MinSeverityInt = $SeverityOrder[$MinSeverity]
$NuGetBase      = "https://api.nuget.org/v3-flatcontainer"
$script:_cache  = @{}

function Write-Banner {
    $line = "=" * 72
    Write-Host ""
    Write-Host $line -ForegroundColor Cyan
    Write-Host "  NuGet Vulnerability Fixer  --  CloudNative Solution" -ForegroundColor Cyan
    Write-Host $line -ForegroundColor Cyan
    Write-Host ""
}

function Write-Section([string]$title) {
    Write-Host ""
    Write-Host "  $title" -ForegroundColor Yellow
    Write-Host ("  " + ("-" * $title.Length)) -ForegroundColor DarkGray
}

function Write-Ok  ([string]$m) { Write-Host "  [OK]  $m" -ForegroundColor Green   }
function Write-Info([string]$m) { Write-Host "  [..]  $m" -ForegroundColor Cyan    }
function Write-Warn([string]$m) { Write-Host "  [!!]  $m" -ForegroundColor Yellow  }
function Write-Fix ([string]$m) { Write-Host "  [FIX] $m" -ForegroundColor Magenta }
function Write-Err ([string]$m) { Write-Host "  [ERR] $m" -ForegroundColor Red     }

function Get-RepoRoot {
    if ($Solution -ne "") { return (Resolve-Path $Solution -ErrorAction Stop).Path }
    $up = $PSScriptRoot
    1..3 | ForEach-Object { $up = Split-Path $up -Parent }
    return $up
}

function Find-Projects {
    if ($Project -ne "") {
        return @((Resolve-Path $Project -ErrorAction Stop).Path)
    }

    $root = Get-RepoRoot

    $slnFile = Get-ChildItem -Path "$root\*" -Include "*.slnx","*.sln" `
                             -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($slnFile) {
        Write-Info "Solution : $($slnFile.Name)"
        return @(Read-ProjectsFromSolution $slnFile.FullName $root)
    }

    Write-Warn "No solution file found — globbing for *.csproj"
    return @(
        Get-ChildItem -Path $root -Recurse -Filter "*.csproj" |
            Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
            Select-Object -ExpandProperty FullName
    )
}

function Read-ProjectsFromSolution([string]$sln, [string]$root) {
    $text = Get-Content $sln -Raw
    $ext  = [IO.Path]::GetExtension($sln).ToLower()

    $relativePaths = if ($ext -eq ".slnx") {
        ([xml]$text).SelectNodes("//Project[@Path]") |
            Where-Object { $_.Path -match '\.csproj$' } |
            Select-Object -ExpandProperty Path
    } else {
        [regex]::Matches($text, '"([^"]+\.csproj)"') |
            ForEach-Object { $_.Groups[1].Value }
    }

    return $relativePaths | ForEach-Object {
        $full = Join-Path $root ($_ -replace '/', '\')
        if (Test-Path $full) { $full }
        else { Write-Warn "Skipped (not on disk): $_"; $null }
    } | Where-Object { $_ }
}

class VulnEntry {
    [string] $PackageId
    [string] $Version
    [string] $Severity
    [string] $AdvisoryUrl
    [string] $ProjectPath
    [bool]   $IsTransitive
}

function Get-VulnerablePackages([string[]]$paths) {
    $results = [Collections.Generic.List[VulnEntry]]::new()
    $seen    = [Collections.Generic.HashSet[string]]::new()

    foreach ($proj in $paths) {
        $name = [IO.Path]::GetFileNameWithoutExtension($proj)
        Write-Info "Scanning  $name ..."

        $cmdArgs = @("list", $proj, "package", "--vulnerable")
        if ($IncludeTransitive) { $cmdArgs += "--include-transitive" }

        $output      = & dotnet @cmdArgs 2>&1
        $inTransient = $false

        foreach ($line in ($output | Where-Object { $_ -is [string] })) {
            if ($line -match 'Transitive\s+Package') { $inTransient = $true;  continue }
            if ($line -match 'Top-level\s+Package')  { $inTransient = $false; continue }

            if ($line -match '^\s+>\s+(\S+)\s+(\S+)\s+\S+\s+(Low|Moderate|High|Critical)\s+(https://\S+)') {
                $sev = $Matches[3]
                if ($SeverityOrder[$sev] -lt $MinSeverityInt) { continue }

                $key = "$proj|$($Matches[1])|$inTransient"
                if ($seen.Add($key)) {
                    $e = [VulnEntry]::new()
                    $e.PackageId    = $Matches[1]
                    $e.Version      = $Matches[2]
                    $e.Severity     = $sev
                    $e.AdvisoryUrl  = $Matches[4]
                    $e.ProjectPath  = $proj
                    $e.IsTransitive = $inTransient
                    $results.Add($e)
                }
            }
        }
    }

    return @($results)
}

function Get-LatestStable([string]$id) {
    if ($script:_cache.ContainsKey($id)) { return $script:_cache[$id] }

    try {
        $url  = "$NuGetBase/$($id.ToLower())/index.json"
        $resp = Invoke-RestMethod -Uri $url -TimeoutSec 15 -ErrorAction Stop

        $stable = $resp.versions |
            Where-Object { $_ -notmatch '-' } |
            Sort-Object   { [System.Version]($_ -replace '^(\d+\.\d+\.\d+).*','$1') } |
            Select-Object -Last 1

        $script:_cache[$id] = $stable
        return $stable
    } catch {
        Write-Warn "NuGet API error for '$id': $_"
        $script:_cache[$id] = $null
        return $null
    }
}

function Compare-SemVer([string]$a, [string]$b) {
    $va = [System.Version]($a -replace '^(\d+\.\d+\.\d+).*','$1')
    $vb = [System.Version]($b -replace '^(\d+\.\d+\.\d+).*','$1')
    return $va.CompareTo($vb)
}

function Update-PackageInFile {
    [OutputType([bool])]
    param(
        [string] $FilePath,
        [string] $PackageId,
        [string] $OldVersion,
        [string] $NewVersion,
        [bool]   $IsCpm = $false
    )

    $element = if ($IsCpm) { "PackageVersion" } else { "PackageReference" }
    $idEsc   = [regex]::Escape($PackageId)
    $verEsc  = [regex]::Escape($OldVersion)

    $lines   = Get-Content $FilePath
    $changed = $false

    $newLines = $lines | ForEach-Object {
        $line = $_
        $hasId    = $line -match "<$element[^>]+Include=`"$idEsc`""
        $hasVer   = $line -match "Version=`"$verEsc`""
        $hasChild = $line -match "<Version>$verEsc</Version>"

        if ($hasId -and ($hasVer -or $hasChild)) {
            $changed = $true
            if ($hasVer) {
                $line -replace "Version=`"$verEsc`"", "Version=`"$NewVersion`""
            } else {
                $line -replace "<Version>$verEsc</Version>", "<Version>$NewVersion</Version>"
            }
        } else {
            $line
        }
    }

    if ($changed) {
        $display = $FilePath -replace [regex]::Escape((Get-RepoRoot) + "\"), ""
        if ($PSCmdlet.ShouldProcess($display, "Update '$PackageId' $OldVersion -> $NewVersion")) {
            $newLines | Set-Content -Path $FilePath -Encoding UTF8
            return $true
        }
        return $false
    }
    return $false
}

function Find-CpmFile {
    if ($CpmFile -ne "" -and (Test-Path $CpmFile)) { return (Resolve-Path $CpmFile).Path }
    $cpm = Join-Path (Get-RepoRoot) "Directory.Packages.props"
    if (Test-Path $cpm) { return $cpm }
    return $null
}

# ── Entry point ──────────────────────────────────────────────────────────────

Write-Banner

Write-Section "Locating Projects"
$projects = Find-Projects
Write-Info "Found $($projects.Count) project(s)"

$cpmPath = Find-CpmFile
if ($cpmPath) { Write-Info "CPM file : $(Split-Path $cpmPath -Leaf)" }

Write-Section "Scanning for Vulnerabilities"
$all          = @(Get-VulnerablePackages $projects)
$directVulns  = @($all | Where-Object { -not $_.IsTransitive })
$transitVulns = @($all | Where-Object {       $_.IsTransitive })

if ($all.Count -eq 0) {
    Write-Ok "No vulnerable packages found at or above '$MinSeverity' severity."
    Write-Host ""
    exit 0
}

Write-Section "Vulnerability Report"

$c1 = 52; $c2 = 10; $c3 = 12
Write-Host ""
Write-Host ("  {0,-$c1} {1,-$c2} {2,-$c3} {3}" -f "Package","Version","Severity","Project") `
    -ForegroundColor Cyan
Write-Host ("  " + ("-" * 95)) -ForegroundColor DarkGray

foreach ($v in ($all | Sort-Object @{E={$SeverityOrder[$_.Severity]};Descending=$true}, PackageId)) {
    $tag   = if ($v.IsTransitive) { " [transitive]" } else { "" }
    $color = $SeverityColors[$v.Severity]
    $proj  = [IO.Path]::GetFileNameWithoutExtension($v.ProjectPath)

    Write-Host ("  {0,-$c1} {1,-$c2}" -f $v.PackageId, $v.Version) -NoNewline -ForegroundColor White
    Write-Host (" {0,-$c3}" -f $v.Severity) -NoNewline -ForegroundColor $color
    Write-Host "$proj$tag" -ForegroundColor DarkGray
}

Write-Host ""
Write-Info "$($directVulns.Count) direct  |  $($transitVulns.Count) transitive"

if ($ScanOnly) {
    Write-Host ""
    Write-Warn "Scan-only mode — no changes applied. Remove -ScanOnly to auto-fix."
    Write-Host ""
    exit 0
}

Write-Section "Resolving Patched Versions from NuGet"

$patchMap = @{}
$directVulns | Select-Object PackageId, Version -Unique | ForEach-Object {
    $latest = Get-LatestStable $_.PackageId

    if (-not $latest) {
        Write-Warn "$($_.PackageId) -- NuGet API error, skipping"
        return
    }

    $cmp = Compare-SemVer $latest $_.Version
    if ($cmp -gt 0) {
        $patchMap[$_.PackageId] = $latest
        Write-Info "$($_.PackageId)  $($_.Version)  ->  $latest"
    } elseif ($cmp -eq 0) {
        Write-Warn "$($_.PackageId) -- already at latest ($latest). May need a manual workaround."
    } else {
        Write-Warn "$($_.PackageId) -- NuGet latest ($latest) < current ($($_.Version)). Skipping."
    }
}

if ($patchMap.Count -eq 0) {
    Write-Warn "No auto-fixable vulnerabilities. All require manual intervention."
    Write-Host ""
    exit 1
}

Write-Section "Applying Fixes"

$fixCount      = 0
$modifiedFiles = [Collections.Generic.HashSet[string]]::new()

foreach ($vuln in $directVulns) {
    if (-not $patchMap.ContainsKey($vuln.PackageId)) { continue }

    $newVer = $patchMap[$vuln.PackageId]
    $target = $vuln.ProjectPath
    $isCpm  = $false

    if ($cpmPath) {
        $cpmContent = Get-Content $cpmPath -Raw
        $idEsc      = [regex]::Escape($vuln.PackageId)
        if ($cpmContent -match "PackageVersion[^>]+Include=`"$idEsc`"") {
            $target = $cpmPath
            $isCpm  = $true
        }
    }

    $ok = Update-PackageInFile `
        -FilePath   $target `
        -PackageId  $vuln.PackageId `
        -OldVersion $vuln.Version `
        -NewVersion $newVer `
        -IsCpm      $isCpm

    if ($ok) {
        $label = [IO.Path]::GetFileNameWithoutExtension($target)
        Write-Fix "[$label]  $($vuln.PackageId)  $($vuln.Version)  ->  $newVer"
        $fixCount++
        [void]$modifiedFiles.Add($target)
    }
}

if ($transitVulns.Count -gt 0) {
    Write-Section "Transitive Vulnerabilities -- Manual Action Required"
    Write-Warn "Add explicit PackageReference overrides to the affected project(s):"
    Write-Host ""

    $transitVulns | Group-Object PackageId | ForEach-Object {
        $pkg = $_.Group[0]
        $ver = Get-LatestStable $pkg.PackageId
        $ver = if ($ver) { $ver } else { "<latest>" }
        $aff = ($_.Group | Select-Object -ExpandProperty ProjectPath |
                    ForEach-Object { [IO.Path]::GetFileNameWithoutExtension($_) }) -join ", "

        Write-Host "  Package  : $($pkg.PackageId)" -ForegroundColor White
        Write-Host "  Severity : $($pkg.Severity)   Advisory: $($pkg.AdvisoryUrl)" -ForegroundColor DarkGray
        Write-Host "  Affected : $aff" -ForegroundColor DarkGray
        Write-Host "  Fix      : Add to affected .csproj (or Directory.Packages.props):" -ForegroundColor DarkGray
        Write-Host "    <PackageReference Include=`"$($pkg.PackageId)`" Version=`"$ver`" />" -ForegroundColor Green
        Write-Host ""
    }
}

if (-not $NoRestore -and $fixCount -gt 0 -and -not $WhatIfPreference) {
    Write-Section "Running dotnet restore"

    $root    = Get-RepoRoot
    $slnFile = Get-ChildItem -Path "$root\*" -Include "*.slnx","*.sln" `
                             -ErrorAction SilentlyContinue | Select-Object -First 1
    $restoreTarget = if ($slnFile) { $slnFile.FullName } else { $root }

    & dotnet restore $restoreTarget --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Err "dotnet restore failed. Review changes manually."
        exit $LASTEXITCODE
    }
    Write-Ok "Restore completed."
}

Write-Section "Summary"
Write-Host ""
if ($WhatIfPreference) {
    Write-Warn "WhatIf mode -- no files were written."
} elseif ($fixCount -gt 0) {
    Write-Ok "$fixCount package version(s) updated across $($modifiedFiles.Count) file(s)."
    Write-Info "Run 'dotnet build' to confirm all NU1902/NU1903 warnings are resolved."
} else {
    Write-Warn "No changes were applied."
}
Write-Host ""
