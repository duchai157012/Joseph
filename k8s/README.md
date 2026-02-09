# Kubernetes Manifests for Joseph Project

This directory contains Kubernetes manifests organized using **Kustomize** for managing multiple environments.

## Directory Structure

```
k8s/
├── base/                          # Base manifests (shared across all environments)
│   ├── config/                    # ConfigMaps and base secrets
│   ├── infrastructure/            # Infrastructure services (DB, message queue, etc.)
│   ├── networking/                # Ingress and networking
│   ├── services/                  # Application services
│   └── kustomization.yaml         # Base kustomization
├── overlays/                      # Environment-specific overlays
│   ├── test/                      # Test environment
│   │   ├── kustomization.yaml
│   │   └── secrets.yaml
│   └── prod/                      # Production environment
│       ├── kustomization.yaml
│       └── secrets.yaml
├── argocd/                        # ArgoCD applications
├── argo-workflows/                # Argo Workflows
└── kind/                          # Kind cluster config

```

## Environments

### Base
Contains all shared resources and default configurations used across all environments.

### Test Environment (`overlays/test`)
- **Namespace:** `joseph-test`
- **Resource Limits:** Lower (256Mi memory, 250m CPU)
- **Replicas:** 1
- **Environment:** Staging

### Production Environment (`overlays/prod`)
- **Namespace:** `joseph-prod`
- **Resource Limits:** Higher (1Gi memory, 1000m CPU)
- **Replicas:** 3 (auto-scaling 3-20)
- **Environment:** Production

## Deployment Commands

### Deploy to Test Environment
```bash
# Preview what will be applied
kubectl kustomize k8s/overlays/test

# Apply to cluster
kubectl apply -k k8s/overlays/test
```

### Deploy to Production Environment
```bash
# Preview what will be applied
kubectl kustomize k8s/overlays/prod

# Apply to cluster
kubectl apply -k k8s/overlays/prod
```

### Deploy Base (Development)
```bash
# For local development using base manifests
kubectl apply -k k8s/base
```

## GitOps with ArgoCD

For production deployments, use GitOps with ArgoCD for automated, declarative deployments.

### Quick Start

1. **Install ArgoCD:**
```bash
cd k8s/argocd
.\install-argocd.ps1
```

2. **Deploy using App of Apps (Recommended):**
```bash
# Apply AppProject
kubectl apply -f k8s/argocd/argocd-project.yaml

# Deploy all environments
kubectl apply -f k8s/argocd/app-of-apps.yaml
```

3. **Access ArgoCD UI:**
```bash
kubectl port-forward svc/argocd-server -n argocd 8080:443
# Username: admin
# Password: kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d
```

### Applications

- **joseph-dev**: Development environment (auto-sync from `k8s/base`)
- **joseph-test**: Test environment (auto-sync from `k8s/overlays/test`)
- **joseph-prod**: Production environment (manual sync from `k8s/overlays/prod`)

**📖 Full GitOps Guide:** [k8s/argocd/GITOPS_GUIDE.md](argocd/GITOPS_GUIDE.md)

## Using with ArgoCD

Update your ArgoCD Application to point to specific overlays:

```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: joseph-prod
spec:
  source:
    repoURL: https://github.com/your-org/joseph
    targetRevision: main
    path: k8s/overlays/prod
  destination:
    server: https://kubernetes.default.svc
    namespace: joseph-prod
  syncPolicy:
    automated:
      prune: true
      selfHeal: true
```

## Customizing Environments

### Adding a New Environment

1. Create a new overlay directory:
```bash
mkdir -p k8s/overlays/staging
```

2. Create `kustomization.yaml`:
```yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

namespace: joseph-staging

bases:
  - ../../base

namePrefix: staging-

commonLabels:
  environment: staging
```

3. Deploy:
```bash
kubectl apply -k k8s/overlays/staging
```

### Modifying Resources per Environment

Use Kustomize patches in the overlay's `kustomization.yaml`:

```yaml
patches:
  - target:
      kind: Deployment
      name: auth-api
    patch: |-
      - op: replace
        path: /spec/replicas
        value: 5
```

## Security Notes

⚠️ **IMPORTANT:** Never commit production secrets to Git!

For production secrets, use:
- **Sealed Secrets**: https://github.com/bitnami-labs/sealed-secrets
- **External Secrets Operator**: https://external-secrets.io/
- **Cloud Provider Solutions**: Azure Key Vault, AWS Secrets Manager, GCP Secret Manager
- **HashiCorp Vault**

## Services Included

- **auth-api**: Authentication service
- **catalog-api**: Catalog management
- **order-api**: Order processing
- **payment-api**: Payment processing (with HPA)
- **notification-api**: Notification service (with HPA)
- **gateway**: API Gateway (YARP reverse proxy)

## Infrastructure Components

- SQL Server (for auth, catalog, order databases)
- RabbitMQ (message broker)
- Redis (caching)
- Seq (logging)
- Zipkin (distributed tracing)

## Quick Start (Local Development)

1. Create a Kind cluster:
```bash
kind create cluster --config k8s/kind/cluster-config.yaml
```

2. Deploy to local cluster:
```bash
kubectl apply -k k8s/base
```

3. Check deployment status:
```bash
kubectl get pods -n joseph-dev
```

4. Access the gateway:
```bash
kubectl port-forward -n joseph-dev svc/gateway 8080:80
```

## Verification

After deployment, verify all services are running:

```bash
# For test environment
kubectl get all -n joseph-test

# For production environment
kubectl get all -n joseph-prod
```
