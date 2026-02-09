# Install kubectl CLI Tool
# This script installs kubectl for Windows using Chocolatey or direct download

param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"

Write-Host "=== kubectl Installation Script ===" -ForegroundColor Cyan
Write-Host ""

# Check if kubectl is already installed
$kubectlExists = Get-Command kubectl -ErrorAction SilentlyContinue

if ($kubectlExists -and -not $Force) {
    $version = kubectl version --client --short 2>$null
    Write-Host "✓ kubectl is already installed: $version" -ForegroundColor Green
    Write-Host "  Use -Force to reinstall" -ForegroundColor Yellow
    exit 0
}

# Check if Chocolatey is installed
$chocoExists = Get-Command choco -ErrorAction SilentlyContinue

if ($chocoExists) {
    Write-Host "Installing kubectl via Chocolatey..." -ForegroundColor Yellow
    try {
        choco install kubernetes-cli -y
        Write-Host "✓ kubectl installed successfully via Chocolatey" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Failed to install via Chocolatey: $_" -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host "Chocolatey not found. Installing kubectl manually..." -ForegroundColor Yellow
    
    # Download kubectl
    $kubectlVersion = (Invoke-RestMethod -Uri "https://dl.k8s.io/release/stable.txt").Trim()
    $downloadUrl = "https://dl.k8s.io/release/$kubectlVersion/bin/windows/amd64/kubectl.exe"
    
    $installPath = "$env:ProgramFiles\kubectl"
    $kubectlPath = "$installPath\kubectl.exe"
    
    # Create directory if it doesn't exist
    if (-not (Test-Path $installPath)) {
        New-Item -ItemType Directory -Path $installPath -Force | Out-Null
    }
    
    Write-Host "Downloading kubectl $kubectlVersion..." -ForegroundColor Yellow
    try {
        Invoke-WebRequest -Uri $downloadUrl -OutFile $kubectlPath
        Write-Host "✓ kubectl downloaded to $kubectlPath" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Failed to download kubectl: $_" -ForegroundColor Red
        exit 1
    }
    
    # Add to PATH
    $currentPath = [Environment]::GetEnvironmentVariable("Path", "Machine")
    if ($currentPath -notlike "*$installPath*") {
        Write-Host "Adding kubectl to system PATH..." -ForegroundColor Yellow
        [Environment]::SetEnvironmentVariable("Path", "$currentPath;$installPath", "Machine")
        $env:Path = "$env:Path;$installPath"
        Write-Host "✓ kubectl added to PATH" -ForegroundColor Green
        Write-Host "  Note: You may need to restart your terminal" -ForegroundColor Yellow
    }
}

# Verify installation
Write-Host ""
Write-Host "Verifying kubectl installation..." -ForegroundColor Yellow
try {
    $version = kubectl version --client --output=json 2>$null | ConvertFrom-Json
    Write-Host "✓ kubectl installed successfully!" -ForegroundColor Green
    Write-Host "  Version: $($version.clientVersion.gitVersion)" -ForegroundColor Cyan
}
catch {
    Write-Host "✗ kubectl verification failed: $_" -ForegroundColor Red
    exit 1
}

# Optional: Configure shell completion
Write-Host ""
Write-Host "To enable kubectl autocompletion in PowerShell, add this to your profile:" -ForegroundColor Cyan
Write-Host "  kubectl completion powershell | Out-String | Invoke-Expression" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Run setup-docker-desktop-k8s.ps1 (easiest)" -ForegroundColor White
Write-Host "  2. Or run setup-kind-cluster.ps1 (for multi-cluster testing)" -ForegroundColor White
