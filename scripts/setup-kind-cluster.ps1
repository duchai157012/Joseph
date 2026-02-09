# Setup Kind Kubernetes Cluster
# This script installs Kind and creates a multi-node cluster for Joseph microservices

$ErrorActionPreference = "Stop"

Write-Host "=== Kind Kubernetes Cluster Setup ===" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Yellow
try {
    docker info > $null 2>&1
    Write-Host "✓ Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker is not running" -ForegroundColor Red
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

# Check if Kind is installed
Write-Host "Checking Kind installation..." -ForegroundColor Yellow
$kindExists = Get-Command kind -ErrorAction SilentlyContinue

if (-not $kindExists) {
    Write-Host "Kind is not installed. Installing via Chocolatey..." -ForegroundColor Yellow
    
    $chocoExists = Get-Command choco -ErrorAction SilentlyContinue
    if (-not $chocoExists) {
        Write-Host "✗ Chocolatey is not installed" -ForegroundColor Red
        Write-Host "  Install Chocolatey from https://chocolatey.org/" -ForegroundColor Yellow
        Write-Host "  Or download Kind manually from https://kind.sigs.k8s.io/docs/user/quick-start/" -ForegroundColor Yellow
        exit 1
    }
    
    try {
        choco install kind -y
        Write-Host "✓ Kind installed successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Failed to install Kind: $_" -ForegroundColor Red
        exit 1
    }
}
else {
    $kindVersion = kind version
    Write-Host "✓ Kind is already installed: $kindVersion" -ForegroundColor Green
}

# Create directories for persistent volumes
Write-Host ""
Write-Host "Creating persistent volume directories..." -ForegroundColor Yellow
$repoRoot = Split-Path -Parent $PSScriptRoot
$volumesPath = Join-Path $repoRoot "kind-volumes"

@("sqlserver", "data") | ForEach-Object {
    $path = Join-Path $volumesPath $_
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path -Force | Out-Null
        Write-Host "  Created: $path" -ForegroundColor Gray
    }
}

# Check if cluster already exists
Write-Host ""
Write-Host "Checking for existing joseph-cluster..." -ForegroundColor Yellow
$existingClusters = kind get clusters 2>$null
if ($existingClusters -contains "joseph-cluster") {
    Write-Host "joseph-cluster already exists" -ForegroundColor Yellow
    $response = Read-Host "Delete and recreate? (y/N)"
    if ($response -eq "y" -or $response -eq "Y") {
        Write-Host "Deleting existing cluster..." -ForegroundColor Yellow
        kind delete cluster --name joseph-cluster
        Write-Host "✓ Cluster deleted" -ForegroundColor Green
    }
    else {
        Write-Host "Using existing cluster" -ForegroundColor Green
        kubectl config use-context kind-joseph-cluster
        kubectl cluster-info
        exit 0
    }
}

# Create Kind cluster
Write-Host ""
Write-Host "Creating Kind cluster (this may take 2-3 minutes)..." -ForegroundColor Yellow
$configPath = Join-Path $repoRoot "k8s\kind\cluster-config.yaml"

try {
    kind create cluster --config $configPath
    Write-Host "✓ Kind cluster created successfully!" -ForegroundColor Green
}
catch {
    Write-Host "✗ Failed to create cluster: $_" -ForegroundColor Red
    exit 1
}

# Wait for cluster to be ready
Write-Host ""
Write-Host "Waiting for cluster to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=Ready nodes --all --timeout=300s

# Install NGINX Ingress Controller
Write-Host ""
Write-Host "Installing NGINX Ingress Controller..." -ForegroundColor Yellow
try {
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml
    Write-Host "✓ NGINX Ingress Controller installed" -ForegroundColor Green
    
    # Wait for ingress controller to be ready
    Write-Host "Waiting for ingress controller to be ready..." -ForegroundColor Yellow
    kubectl wait --namespace ingress-nginx `
        --for=condition=ready pod `
        --selector=app.kubernetes.io/component=controller `
        --timeout=300s
    Write-Host "✓ Ingress controller is ready" -ForegroundColor Green
}
catch {
    Write-Host "⚠ Warning: Failed to install NGINX Ingress Controller" -ForegroundColor Yellow
    Write-Host "  You can install it manually later if needed" -ForegroundColor Gray
}

# Display cluster info
Write-Host ""
Write-Host "Cluster Information:" -ForegroundColor Cyan
kubectl cluster-info --context kind-joseph-cluster

Write-Host ""
Write-Host "Nodes:" -ForegroundColor Cyan
kubectl get nodes

Write-Host ""
Write-Host "✓ Kind cluster is ready!" -ForegroundColor Green
Write-Host ""
Write-Host "Cluster name: joseph-cluster" -ForegroundColor Cyan
Write-Host "Context: kind-joseph-cluster" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Run verify-cluster.ps1 to test the cluster" -ForegroundColor White
Write-Host "  2. Proceed with Argo CD and Argo Workflows installation" -ForegroundColor White
Write-Host ""
Write-Host "Useful commands:" -ForegroundColor Cyan
Write-Host "  kind get clusters              - List all clusters" -ForegroundColor White
Write-Host "  kubectl get nodes              - Show cluster nodes" -ForegroundColor White
Write-Host "  kind delete cluster --name joseph-cluster - Delete this cluster" -ForegroundColor White
