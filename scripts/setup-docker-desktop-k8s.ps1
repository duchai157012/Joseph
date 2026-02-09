# Setup Docker Desktop Kubernetes
# This script helps verify and configure Docker Desktop Kubernetes cluster

$ErrorActionPreference = "Stop"

Write-Host "=== Docker Desktop Kubernetes Setup ===" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
Write-Host "Checking Docker Desktop status..." -ForegroundColor Yellow
try {
    docker info > $null 2>&1
    Write-Host "✓ Docker Desktop is running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker Desktop is not running" -ForegroundColor Red
    Write-Host "  Please start Docker Desktop and try again" -ForegroundColor Yellow
    exit 1
}

# Check if kubectl is installed
$kubectlExists = Get-Command kubectl -ErrorAction SilentlyContinue
if (-not $kubectlExists) {
    Write-Host "✗ kubectl is not installed" -ForegroundColor Red
    Write-Host "  Run install-kubectl.ps1 first" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Docker Desktop Kubernetes Setup Instructions:" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Open Docker Desktop" -ForegroundColor White
Write-Host "2. Click on the Settings/Preferences icon (gear)" -ForegroundColor White
Write-Host "3. Navigate to 'Kubernetes' section" -ForegroundColor White
Write-Host "4. Check the box 'Enable Kubernetes'" -ForegroundColor White
Write-Host "5. Click 'Apply & Restart'" -ForegroundColor White
Write-Host "6. Wait 2-5 minutes for Kubernetes to start" -ForegroundColor White
Write-Host ""
Write-Host "Press Enter after you've completed these steps..." -ForegroundColor Yellow
Read-Host

# Wait for Kubernetes to be available
Write-Host ""
Write-Host "Waiting for Kubernetes cluster to be ready..." -ForegroundColor Yellow

$maxAttempts = 30
$attempt = 0
$ready = $false

while ($attempt -lt $maxAttempts -and -not $ready) {
    $attempt++
    try {
        $clusterInfo = kubectl cluster-info 2>&1
        if ($clusterInfo -match "Kubernetes control plane") {
            $ready = $true
            break
        }
    }
    catch {
        # Cluster not ready yet
    }
    
    Write-Host "  Attempt $attempt/$maxAttempts..." -ForegroundColor Gray
    Start-Sleep -Seconds 5
}

if (-not $ready) {
    Write-Host "✗ Kubernetes cluster did not become ready in time" -ForegroundColor Red
    Write-Host "  Please check Docker Desktop settings" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ Kubernetes cluster is running!" -ForegroundColor Green
Write-Host ""

# Set context to docker-desktop
Write-Host "Setting kubectl context to docker-desktop..." -ForegroundColor Yellow
kubectl config use-context docker-desktop

# Verify cluster
Write-Host ""
Write-Host "Cluster Information:" -ForegroundColor Cyan
kubectl cluster-info

Write-Host ""
Write-Host "Nodes:" -ForegroundColor Cyan
kubectl get nodes

Write-Host ""
Write-Host "Namespaces:" -ForegroundColor Cyan
kubectl get namespaces

Write-Host ""
Write-Host "✓ Docker Desktop Kubernetes is ready!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Run verify-cluster.ps1 to test the cluster" -ForegroundColor White
Write-Host "  2. Proceed with Argo CD and Argo Workflows installation" -ForegroundColor White
