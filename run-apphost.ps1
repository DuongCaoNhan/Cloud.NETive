# PowerShell Script to run the AppHost with proper configuration
Write-Host "Starting CloudNative Application Host..." -ForegroundColor Cyan
Write-Host ""
Write-Host "Using HTTP mode to avoid port conflicts and certificate issues" -ForegroundColor Yellow
Write-Host ""

# Kill any existing dotnet processes that might be holding ports
Write-Host "Stopping any existing AppHost/dotnet processes..." -ForegroundColor Magenta
$appHostProcs = Get-Process -Name "CloudNative.AppHost" -ErrorAction SilentlyContinue
if ($appHostProcs) {
    Write-Host "  Killing $($appHostProcs.Count) CloudNative.AppHost processes..." -ForegroundColor Yellow
    $appHostProcs | Stop-Process -Force -ErrorAction SilentlyContinue
}
$procs = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($procs) {
    Write-Host "  Killing $($procs.Count) dotnet processes..." -ForegroundColor Yellow
    $procs | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    # Verify cleanup
    $remaining = (Get-Process -Name "dotnet" -ErrorAction SilentlyContinue).Count
    if ($remaining -gt 0) {
        Write-Host "  WARNING: $remaining processes still running, retrying..." -ForegroundColor Red
        Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
        Start-Sleep -Seconds 2
    }
    Write-Host "  Cleanup done." -ForegroundColor Green
} elseif (-not $appHostProcs) {
    Write-Host "  No existing processes found." -ForegroundColor Green
}

# Change to AppHost directory
Set-Location -Path "$PSScriptRoot\src\Orchestrator\CloudNative.AppHost"

# Set environment variables and run with HTTP profile
$env:ASPNETCORE_URLS = "http://localhost:15002"
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = "true"
$env:ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL = "http://localhost:15003"
Write-Host "Starting AppHost..." -ForegroundColor Green
dotnet run --launch-profile http

# Wait before exiting
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
