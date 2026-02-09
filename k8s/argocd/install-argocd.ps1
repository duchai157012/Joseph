# Install Argo CD
# This script downloads and installs Argo CD to the cluster

$ErrorActionPreference = "Stop"

Write-Host "=== Argo CD Installation ===" -ForegroundColor Cyan
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
    Write-Host "  Make sure your cluster is running" -ForegroundColor Yellow
    exit 1
}

# Create argocd namespace
Write-Host ""
Write-Host "Creating argocd namespace..." -ForegroundColor Yellow
kubectl create namespace argocd --dry-run=client -o yaml | kubectl apply -f -
Write-Host "✓ Namespace created" -ForegroundColor Green

# Install Argo CD
Write-Host ""
Write-Host "Installing Argo CD..." -ForegroundColor Yellow
Write-Host "  This may take 2-3 minutes..." -ForegroundColor Gray

try {
    kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
    Write-Host "✓ Argo CD installed" -ForegroundColor Green
}
catch {
    Write-Host "✗ Failed to install Argo CD: $_" -ForegroundColor Red
    exit 1
}

# Wait for Argo CD to be ready
Write-Host ""
Write-Host "Waiting for Argo CD pods to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=Ready pods --all -n argocd --timeout=300s

Write-Host "✓ Argo CD is ready!" -ForegroundColor Green

# Get initial admin password
Write-Host ""
Write-Host "Retrieving initial admin password..." -ForegroundColor Yellow
Start-Sleep -Seconds 5  # Give a moment for the secret to be created

try {
    $argoPassword = kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | ForEach-Object { [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($_)) }
    
    Write-Host ""
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host "Argo CD Credentials" -ForegroundColor Cyan
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host "Username: admin" -ForegroundColor White
    Write-Host "Password: $argoPassword" -ForegroundColor White
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "⚠ Save this password! The secret will be deleted after first login." -ForegroundColor Yellow
}
catch {
    Write-Host "⚠ Could not retrieve initial password" -ForegroundColor Yellow
    Write-Host "  You can get it later with:" -ForegroundColor Gray
    Write-Host '  kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d' -ForegroundColor Gray
}

# Port-forward instructions
Write-Host ""
Write-Host "To access Argo CD UI:" -ForegroundColor Cyan
Write-Host "  1. Run: kubectl port-forward svc/argocd-server -n argocd 8081:443" -ForegroundColor White
Write-Host "  2. Open: https://localhost:8081" -ForegroundColor White
Write-Host "  3. Login with admin / <password above>" -ForegroundColor White
Write-Host ""
Write-Host "Or start port-forward now? (y/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq "y" -or $response -eq "Y") {
    Write-Host ""
    Write-Host "Starting port-forward to Argo CD..." -ForegroundColor Yellow
    Write-Host "Access UI at: https://localhost:8081" -ForegroundColor Cyan
    Write-Host "Press Ctrl+C to stop" -ForegroundColor Gray
    Write-Host ""
    kubectl port-forward svc/argocd-server -n argocd 8081:443
}
else {
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Access the Argo CD UI using port-forward" -ForegroundColor White
    Write-Host "  2. Install Argo CD CLI: choco install argocd-cli" -ForegroundColor White
    Write-Host "  3. Create Argo CD applications for Joseph services" -ForegroundColor White
}
