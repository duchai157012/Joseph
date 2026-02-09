# One-command deployment script for Joseph microservices

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("dev", "staging", "prod")]
    [string]$Environment = "dev"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Joseph Microservices Deployment ===" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host ""

$namespace = "joseph-$Environment"
$repoRoot = Split-Path -Parent $PSScriptRoot

# Check kubectl
$kubectlExists = Get-Command kubectl -ErrorAction SilentlyContinue
if (-not $kubectlExists) {
    Write-Host "✗ kubectl is not installed" -ForegroundColor Red
    Write-Host "  Run .\scripts\install-kubectl.ps1 first" -ForegroundColor Yellow
    exit 1
}

# Check cluster connectivity
Write-Host "Checking cluster connectivity..." -ForegroundColor Yellow
try {
    kubectl cluster-info > $null 2>&1
    $context = kubectl config current-context
    Write-Host "✓ Connected to cluster: $context" -ForegroundColor Green
}
catch {
    Write-Host "✗ Cannot connect to cluster" -ForegroundColor Red
    exit 1
}

# Confirm deployment
Write-Host ""
Write-Host "This will deploy Joseph microservices to namespace: $namespace" -ForegroundColor Yellow
Write-Host ""
Write-Host "Components:" -ForegroundColor Cyan
Write-Host "  • Infrastructure (SQL Server, RabbitMQ, Redis, Seq, Zipkin)" -ForegroundColor White
Write-Host "  • Services (Auth, Catalog, Order, Payment, Notification)" -ForegroundColor White
Write-Host "  • Gateway (YARP reverse proxy)" -ForegroundColor White
Write-Host "  • Networking (Ingress)" -ForegroundColor White
Write-Host ""
Write-Host "Continue? (y/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -ne "y" -and $response -ne "Y") {
    Write-Host "Deployment cancelled" -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Starting deployment..." -ForegroundColor Cyan
Write-Host ""

# Step 1: Create namespace
Write-Host "[1/6] Creating namespace..." -ForegroundColor Yellow
kubectl apply -f "$repoRoot\k8s\base\infrastructure\namespace.yaml"
Write-Host "✓ Namespace created" -ForegroundColor Green

# Step 2: Apply configuration
Write-Host ""
Write-Host "[2/6] Applying configuration..." -ForegroundColor Yellow
kubectl apply -f "$repoRoot\k8s\base\config\"
Write-Host "✓ Configuration applied" -ForegroundColor Green

# Step 3: Deploy infrastructure
Write-Host ""
Write-Host "[3/6] Deploying infrastructure..." -ForegroundColor Yellow
kubectl apply -f "$repoRoot\k8s\base\infrastructure\"
Write-Host "  Waiting for infrastructure to be ready..." -ForegroundColor Gray

# Wait for SQL Server
kubectl wait --for=condition=Ready pod -l app=sqlserver -n $namespace --timeout=300s 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ SQL Server is ready" -ForegroundColor Green
}

# Wait for RabbitMQ
kubectl wait --for=condition=Ready pod -l app=rabbitmq -n $namespace --timeout=300s 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ RabbitMQ is ready" -ForegroundColor Green
}

Write-Host "✓ Infrastructure deployed" -ForegroundColor Green

# Step 4: Deploy services
Write-Host ""
Write-Host "[4/6] Deploying services..." -ForegroundColor Yellow
kubectl apply -f "$repoRoot\k8s\base\services\"
Write-Host "  Waiting for services to be ready..." -ForegroundColor Gray
Start-Sleep -Seconds 10
Write-Host "✓ Services deployed" -ForegroundColor Green

# Step 5: Deploy networking
Write-Host ""
Write-Host "[5/6] Deploying networking..." -ForegroundColor Yellow
kubectl apply -f "$repoRoot\k8s\base\networking\"
Write-Host "✓ Networking deployed" -ForegroundColor Green

# Step 6: Verify deployment
Write-Host ""
Write-Host "[6/6] Verifying deployment..." -ForegroundColor Yellow
Write-Host ""

Write-Host "Pods:" -ForegroundColor Cyan
kubectl get pods -n $namespace

Write-Host ""
Write-Host "Services:" -ForegroundColor Cyan
kubectl get svc -n $namespace

Write-Host ""
Write-Host "Ingress:" -ForegroundColor Cyan
kubectl get ingress -n $namespace

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "✓ Deployment Complete!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Access your services:" -ForegroundColor Cyan
Write-Host "  Gateway: kubectl port-forward -n $namespace svc/gateway 8080:80" -ForegroundColor White
Write-Host "  Seq Logs: kubectl port-forward -n $namespace svc/seq 5341:80" -ForegroundColor White
Write-Host "  RabbitMQ: kubectl port-forward -n $namespace svc/rabbitmq 15672:15672" -ForegroundColor White
Write-Host ""
Write-Host "Useful commands:" -ForegroundColor Cyan
Write-Host "  kubectl get all -n $namespace" -ForegroundColor White
Write-Host "  kubectl logs -n $namespace -l app=order-api --tail=100" -ForegroundColor White
Write-Host "  kubectl describe pod <pod-name> -n $namespace" -ForegroundColor White
