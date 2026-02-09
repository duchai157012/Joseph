# Install Argo Workflows
# This script downloads and installs Argo Workflows to the cluster

$ErrorActionPreference = "Stop"

Write-Host "=== Argo Workflows Installation ===" -ForegroundColor Cyan
Write-Host ""

# Check if kubectl is available
$kubectlExists = Get-Command kubectl -ErrorAction SilentlyContinue
if (-not $kubectlExists) {
    Write-Host "✗ kubectl is not installed" -ForegroundColor Red
    exit 1
}

# Check cluster connectivity
Write-Host "Checking cluster connectivity..." -ForegroundColor Yellow
try {
    kubectl cluster-info > $null 2>&1
    Write-Host "✓ Connected to cluster" -ForegroundColor Green
}
catch {
    Write-Host "✗ Cannot connect to cluster" -ForegroundColor Red
    exit 1
}

# Create argo namespace
Write-Host ""
Write-Host "Creating argo namespace..." -ForegroundColor Yellow
kubectl create namespace argo --dry-run=client -o yaml | kubectl apply -f -
Write-Host "✓ Namespace created" -ForegroundColor Green

# Install Argo Workflows
Write-Host ""
Write-Host "Installing Argo Workflows..." -ForegroundColor Yellow
Write-Host "  This may take 2-3 minutes..." -ForegroundColor Gray

try {
    kubectl apply -n argo -f https://github.com/argoproj/argo-workflows/releases/latest/download/install.yaml
    Write-Host "✓ Argo Workflows installed" -ForegroundColor Green
}
catch {
    Write-Host "✗ Failed to install Argo Workflows: $_" -ForegroundColor Red
    exit 1
}

# Wait for Argo Workflows to be ready
Write-Host ""
Write-Host "Waiting for Argo Workflows pods to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=Ready pods --all -n argo --timeout=300s

Write-Host "✓ Argo Workflows is ready!" -ForegroundColor Green

# Patch argo-server to use server auth mode (for quick start)
Write-Host ""
Write-Host "Configuring Argo Workflows server..." -ForegroundColor Yellow
kubectl patch deployment argo-server -n argo --type='json' -p='[{"op": "add", "path": "/spec/template/spec/containers/0/args/-", "value": "--auth-mode=server"}]'
Write-Host "✓ Server configured with server auth mode" -ForegroundColor Green

# Wait for server to restart
Write-Host "Waiting for server to restart..." -ForegroundColor Yellow
Start-Sleep -Seconds 10
kubectl wait --for=condition=Ready pods -l app=argo-server -n argo --timeout=120s

# Port-forward instructions
Write-Host ""
Write-Host "To access Argo Workflows UI:" -ForegroundColor Cyan
Write-Host "  1. Run: kubectl port-forward -n argo svc/argo-server 2746:2746" -ForegroundColor White
Write-Host "  2. Open: http://localhost:2746" -ForegroundColor White
Write-Host ""
Write-Host "Or start port-forward now? (y/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq "y" -or $response -eq "Y") {
    Write-Host ""
    Write-Host "Starting port-forward to Argo Workflows..." -ForegroundColor Yellow
    Write-Host "Access UI at: http://localhost:2746" -ForegroundColor Cyan
    Write-Host "Press Ctrl+C to stop" -ForegroundColor Gray
    Write-Host ""
    kubectl port-forward -n argo svc/argo-server 2746:2746
}
else {
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Access the Argo Workflows UI using port-forward" -ForegroundColor White
    Write-Host "  2. Install Argo CLI: choco install argo" -ForegroundColor White
    Write-Host "  3. Create workflow templates for Joseph services" -ForegroundColor White
}
