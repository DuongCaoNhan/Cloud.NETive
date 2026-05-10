# Package Management

Automation scripts for NuGet package updates, security scanning, and health checks.

## Scripts

### fix-vulnerable-packages.ps1 — Main Tool

Auto-detects and fixes NuGet vulnerability warnings (`NU1902`, `NU1903`) across the entire solution.

**How it works:**
1. Reads all `.csproj` paths from the `.slnx` / `.sln` file
2. Runs `dotnet list package --vulnerable` per project
3. Queries the NuGet API for the latest stable (non-preview) version
4. Rewrites `PackageReference` / `PackageVersion` entries in place
5. Runs `dotnet restore` to verify the fix

```powershell
# Auto-fix all Moderate+ vulnerabilities
.\fix-vulnerable-packages.ps1

# Preview changes without writing
.\fix-vulnerable-packages.ps1 -WhatIf

# Report only, no changes
.\fix-vulnerable-packages.ps1 -ScanOnly

# Fix only High/Critical, include transitive deps in report
.\fix-vulnerable-packages.ps1 -MinSeverity High -IncludeTransitive

# Target a single project
.\fix-vulnerable-packages.ps1 -Project src\BuildingBlocks\CloudNative.ServiceDefaults\CloudNative.ServiceDefaults.csproj
```

**Parameters:**

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-Solution` | repo root | Path to `.slnx` / `.sln` or repo root directory |
| `-Project` | — | Scan a single `.csproj` only |
| `-ScanOnly` | `$false` | Report without writing changes |
| `-WhatIf` | `$false` | Preview changes without writing |
| `-IncludeTransitive` | `$false` | Also list transitive vulnerabilities |
| `-MinSeverity` | `Moderate` | Minimum severity: `Low` \| `Moderate` \| `High` \| `Critical` |
| `-NoRestore` | `$false` | Skip `dotnet restore` after fixes |
| `-CpmFile` | auto-detect | Path to `Directory.Packages.props` |

---

### security-scan.ps1 — Vulnerability Scanner

Thin wrapper that runs `fix-vulnerable-packages.ps1 -ScanOnly`. No files are changed.

```powershell
.\security-scan.ps1
.\security-scan.ps1 -MinSeverity High -IncludeTransitive
```

---

### update-opentelemetry.ps1 — OpenTelemetry Updater

Fixes vulnerable OpenTelemetry packages by delegating to the main tool.
Primary OpenTelemetry versions are pinned in `CloudNative.ServiceDefaults.csproj`.

```powershell
.\update-opentelemetry.ps1
.\update-opentelemetry.ps1 -ScanOnly
.\update-opentelemetry.ps1 -WhatIf
```

---

### package-health-check.ps1 — Full Health Report

Runs both an outdated-package check and a vulnerability scan.

```powershell
.\package-health-check.ps1
.\package-health-check.ps1 -SkipOutdatedCheck      # vulnerability scan only
.\package-health-check.ps1 -SkipVulnerabilityScan  # outdated check only
```

---

## Common Package Versions

Key packages pinned in `CloudNative.ServiceDefaults.csproj`:

| Package | Version |
|---------|---------|
| `OpenTelemetry.Extensions.Hosting` | 1.15.3 |
| `OpenTelemetry.Exporter.OpenTelemetryProtocol` | 1.15.3 |
| `OpenTelemetry.Instrumentation.AspNetCore` | 1.15.2 |
| `OpenTelemetry.Instrumentation.Http` | 1.15.1 |
| `OpenTelemetry.Instrumentation.Runtime` | 1.15.1 |
| `Serilog.AspNetCore` | 10.0.0 |
| `Swashbuckle.AspNetCore` | 10.0.1 |
| `Aspire.Microsoft.EntityFrameworkCore.SqlServer` | 13.1.1 |
