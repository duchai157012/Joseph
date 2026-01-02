# GitHub Actions CI/CD Setup Guide

## Overview
This workflow automates building, testing, dockerizing, and deploying all 6 microservices in the Joseph solution.

## Workflow Jobs

### 1. Build & Test (Always runs)
- Restores NuGet packages with caching
- Builds entire solution in Release mode
- Runs all unit tests
- Uploads test results as artifacts

### 2. Dockerize (Runs on push to main/develop)
- Builds Docker images for all 6 services in parallel
- Pushes to GitHub Container Registry (ghcr.io)
- Uses Docker layer caching for faster builds
- Tags images with branch name, SHA, and 'latest'

### 3. Deploy to Azure (Runs on push to main only)
- Deploys all services to Azure Container Apps
- Requires production environment approval
- Sets production environment variables

### 4. Deploy to Docker Hub (Alternative - Optional)
- Only runs if variable `USE_DOCKER_HUB=true` is set
- Pushes images to Docker Hub instead of GHCR

---

## Required Secrets Setup

Navigate to your GitHub repository → **Settings** → **Secrets and variables** → **Actions**

### Required Secrets:

#### For GitHub Container Registry (Default - Free)
No secrets needed! Uses `GITHUB_TOKEN` automatically.

#### For Azure Deployment (Optional)
1. **AZURE_CREDENTIALS**
   ```json
   {
     "clientId": "<your-client-id>",
     "clientSecret": "<your-client-secret>",
     "subscriptionId": "<your-subscription-id>",
     "tenantId": "<your-tenant-id>"
   }
   ```
   **How to get:**
   ```bash
   az ad sp create-for-rbac --name "joseph-github-actions" \
     --role contributor \
     --scopes /subscriptions/<subscription-id>/resourceGroups/<resource-group> \
     --sdk-auth
   ```

2. **AZURE_RESOURCE_GROUP**
   - Value: Your Azure resource group name (e.g., `joseph-rg`)

3. **SEQ_URL**
   - Value: Your Seq logging endpoint (e.g., `https://your-seq.com`)

#### For Docker Hub Deployment (Alternative - Optional)
1. **DOCKERHUB_USERNAME**
   - Value: Your Docker Hub username

2. **DOCKERHUB_TOKEN**
   - Value: Docker Hub access token
   - **How to get:** Docker Hub → Account Settings → Security → New Access Token

---

## Repository Variables Setup

For Docker Hub alternative, add this variable:

**Settings** → **Secrets and variables** → **Actions** → **Variables** tab:

- **USE_DOCKER_HUB** = `true` (to enable Docker Hub push)

---

## How It Works

### On Pull Request:
```
✓ Build & Test only
  → Validates code quality
  → Ensures tests pass
```

### On Push to main:
```
✓ Build & Test
  ↓
✓ Dockerize (all 6 services in parallel)
  ↓
✓ Deploy to Azure Container Apps (with approval)
```

### On Push to develop:
```
✓ Build & Test
  ↓
✓ Dockerize (tagged as 'develop-<sha>')
```

---

## Accessing Your Docker Images

### From GitHub Container Registry:
```bash
# Pull auth-api image
docker pull ghcr.io/<your-username>/joseph-auth-api:latest

# Pull specific version by SHA
docker pull ghcr.io/<your-username>/joseph-auth-api:main-abc1234
```

### From Docker Hub (if enabled):
```bash
docker pull <dockerhub-username>/joseph-auth-api:latest
```

---

## Setting Up Azure Container Apps (First Time)

```bash
# Login to Azure
az login

# Create resource group
az group create --name joseph-rg --location eastus

# Create Container Apps environment
az containerapp env create \
  --name joseph-env \
  --resource-group joseph-rg \
  --location eastus

# Create each container app (example for auth-api)
az containerapp create \
  --name joseph-auth-api \
  --resource-group joseph-rg \
  --environment joseph-env \
  --image ghcr.io/<your-username>/joseph-auth-api:latest \
  --target-port 8080 \
  --ingress external \
  --registry-server ghcr.io \
  --registry-username <your-github-username> \
  --registry-password <github-token> \
  --env-vars \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection=secretref:db-connection \
    Logging__SeqUrl=<seq-url>

# Repeat for all 6 services
```

---

## Monitoring Workflow Runs

1. Go to your repository on GitHub
2. Click the **Actions** tab
3. View workflow runs, logs, and test results

---

## Customization Options

### Change Target Branch
Edit `.github/workflows/ci-cd.yml`:
```yaml
on:
  push:
    branches: [ main, develop, staging ]  # Add more branches
```

### Add Environment Protection
1. **Settings** → **Environments** → **New environment** → `production`
2. Add required reviewers before deployment
3. Set deployment branch restrictions

### Modify Docker Registry
To use Azure Container Registry instead:
```yaml
- name: Log in to Azure Container Registry
  uses: docker/login-action@v3
  with:
    registry: <your-acr>.azurecr.io
    username: ${{ secrets.ACR_USERNAME }}
    password: ${{ secrets.ACR_PASSWORD }}
```

---

## Cost Optimization

- **GitHub Actions minutes:** 2,000 free minutes/month on public repos, 3,000 on private repos
- **GitHub Container Registry:** Free for public packages
- **Docker layer caching:** Saves ~5-10 minutes per build
- **NuGet package caching:** Saves ~2-3 minutes per build

---

## Troubleshooting

### Build fails with "Unable to load assembly"
- Run `dotnet restore` locally first
- Check .csproj references are correct

### Docker push permission denied
- For GHCR: Ensure `packages: write` permission is set in workflow
- For Docker Hub: Verify DOCKERHUB_TOKEN is valid

### Azure deployment fails
- Verify AZURE_CREDENTIALS is valid JSON
- Check resource group and container app exist
- Ensure service principal has Contributor role

---

## Next Steps

1. **Add this workflow file to your repository**
2. **Set up required secrets** in GitHub Settings
3. **Push to main branch** to trigger first deployment
4. **Monitor the Actions tab** for workflow status
5. **Set up Production environment protection** for manual approval

Your microservices will now automatically build, test, and deploy on every push! 🚀
