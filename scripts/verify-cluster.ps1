# Verify Kubernetes Cluster
# This script runs various checks to ensure the cluster is operational

$ErrorActionPreference = "Stop"

Write-Host "=== Kubernetes Cluster Verification ===" -ForegroundColor Cyan
Write-Host ""

$allPassed = $true

# Test 1: kubectl is installed
Write-Host "[1/6] Checking kubectl installation..." -ForegroundColor Yellow
try {
    $version = kubectl version --client --output=json 2>$null | ConvertFrom-Json
    Write-Host "  ✓ kubectl version: $($version.clientVersion.gitVersion)" -ForegroundColor Green
}
catch {
    Write-Host "  ✗ kubectl is not installed or not working" -ForegroundColor Red
    $allPassed = $false
}

# Test 2: Cluster connectivity
Write-Host ""
Write-Host "[2/6] Checking cluster connectivity..." -ForegroundColor Yellow
try {
    $clusterInfo = kubectl cluster-info 2>&1
    if ($clusterInfo -match "Kubernetes control plane") {
        Write-Host "  ✓ Connected to cluster" -ForegroundColor Green
        $context = kubectl config current-context
        Write-Host "  Current context: $context" -ForegroundColor Cyan
    }
    else {
        Write-Host "  ✗ Cannot connect to cluster" -ForegroundColor Red
        $allPassed = $false
    }
}
catch {
    Write-Host "  ✗ Cluster connection failed: $_" -ForegroundColor Red
    $allPassed = $false
}

# Test 3: Nodes are ready
Write-Host ""
Write-Host "[3/6] Checking node status..." -ForegroundColor Yellow
try {
    $nodes = kubectl get nodes -o json | ConvertFrom-Json
    $readyNodes = 0
    $totalNodes = $nodes.items.Count
    
    foreach ($node in $nodes.items) {
        $nodeName = $node.metadata.name
        $conditions = $node.status.conditions | Where-Object { $_.type -eq "Ready" }
        
        if ($conditions.status -eq "True") {
            Write-Host "  ✓ Node $nodeName is Ready" -ForegroundColor Green
            $readyNodes++
        }
        else {
            Write-Host "  ✗ Node $nodeName is NOT Ready" -ForegroundColor Red
            $allPassed = $false
        }
    }
    
    Write-Host "  Summary: $readyNodes/$totalNodes nodes ready" -ForegroundColor Cyan
}
catch {
    Write-Host "  ✗ Failed to check nodes: $_" -ForegroundColor Red
    $allPassed = $false
}

# Test 4: Default namespaces exist
Write-Host ""
Write-Host "[4/6] Checking default namespaces..." -ForegroundColor Yellow
try {
    $namespaces = kubectl get namespaces -o json | ConvertFrom-Json
    $requiredNS = @("default", "kube-system", "kube-public", "kube-node-lease")
    
    foreach ($ns in $requiredNS) {
        $exists = $namespaces.items | Where-Object { $_.metadata.name -eq $ns }
        if ($exists) {
            Write-Host "  ✓ Namespace '$ns' exists" -ForegroundColor Green
        }
        else {
            Write-Host "  ✗ Namespace '$ns' is missing" -ForegroundColor Red
            $allPassed = $false
        }
    }
}
catch {
    Write-Host "  ✗ Failed to check namespaces: $_" -ForegroundColor Red
    $allPassed = $false
}

# Test 5: Create a test pod
Write-Host ""
Write-Host "[5/6] Testing pod creation..." -ForegroundColor Yellow
try {
    # Create a simple test pod
    $testPod = @"
apiVersion: v1
kind: Pod
metadata:
  name: cluster-test-pod
  namespace: default
spec:
  containers:
  - name: test
    image: busybox:latest
    command: ['sh', '-c', 'echo "Cluster test successful!" && sleep 10']
  restartPolicy: Never
"@
    
    $testPod | kubectl apply -f - > $null 2>&1
    
    # Wait for pod to start
    Start-Sleep -Seconds 3
    
    # Check pod status
    $podStatus = kubectl get pod cluster-test-pod -n default -o jsonpath='{.status.phase}' 2>$null
    
    if ($podStatus -eq "Running" -or $podStatus -eq "Succeeded") {
        Write-Host "  ✓ Test pod created and running" -ForegroundColor Green
    }
    else {
        Write-Host "  ⚠ Test pod created but status is: $podStatus" -ForegroundColor Yellow
    }
    
    # Get logs
    $logs = kubectl logs cluster-test-pod -n default 2>$null
    if ($logs -match "Cluster test successful") {
        Write-Host "  ✓ Test pod logs verified" -ForegroundColor Green
    }
}
catch {
    Write-Host "  ⚠ Pod creation test had issues: $_" -ForegroundColor Yellow
}

# Test 6: Cleanup test resources
Write-Host ""
Write-Host "[6/6] Cleaning up test resources..." -ForegroundColor Yellow
try {
    kubectl delete pod cluster-test-pod -n default --ignore-not-found=true > $null 2>&1
    Write-Host "  ✓ Test resources cleaned up" -ForegroundColor Green
}
catch {
    Write-Host "  ⚠ Cleanup had issues (not critical)" -ForegroundColor Yellow
}

# Final summary
Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
if ($allPassed) {
    Write-Host "✓ All checks passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Your Kubernetes cluster is ready for:" -ForegroundColor Cyan
    Write-Host "  • Deploying Joseph microservices" -ForegroundColor White
    Write-Host "  • Installing Argo CD" -ForegroundColor White
    Write-Host "  • Installing Argo Workflows" -ForegroundColor White
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Review the implementation plan" -ForegroundColor White
    Write-Host "  2. Create Kubernetes manifests for infrastructure" -ForegroundColor White
    Write-Host "  3. Install Argo CD and Argo Workflows" -ForegroundColor White
}
else {
    Write-Host "✗ Some checks failed" -ForegroundColor Red
    Write-Host "  Please review the errors above and fix them before proceeding" -ForegroundColor Yellow
}
Write-Host "================================" -ForegroundColor Cyan
