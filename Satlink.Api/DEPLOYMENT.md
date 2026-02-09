# Satlink.Api - Deployment / CI-CD

This document describes how to set up **CI/CD** for `Satlink.Api` with GitHub Actions, including:
- CI: build + tests + coverage + SonarQube.
- CD: Docker build + push to ACR + deploy to Azure App Service (container) on `main`.

> Workflows:
> - `.github/workflows/satlink-api-ci.yml`
> - `.github/workflows/satlink-api-cd.yml`

## 1. Requirements

- GitHub repository.
- Azure Subscription.
- Azure Container Registry (ACR).
- Azure App Service (Linux) configured to run a container.
- GitHub Actions enabled.

## 2. Required secrets (GitHub)

Configure in: **Repository Settings → Secrets and variables → Actions**.

### 2.1. SonarQube (CI)

The analysis runs only if the secrets exist and are not empty.

- `SONAR_HOST_URL`
  - Example: `https://sonarqube.company.com`
- `SONAR_TOKEN`
  - Access token generated in SonarQube.
- `SONAR_PROJECT_KEY`
  - Project key in SonarQube.

### 2.2. Azure (CD)

Login uses OIDC (`azure/login@v2`).

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

> These values come from an App Registration / Service Principal with federated credentials for GitHub.

### 2.3. Azure Container Registry (ACR)

- `ACR_NAME`
  - ACR name (without FQDN), e.g. `satlinkacr`.
- `ACR_LOGIN_SERVER`
  - Login server, e.g. `satlinkacr.azurecr.io`.
- `ACR_REPOSITORY`
  - Image repository inside ACR, e.g. `satlink-api`.

### 2.4. Azure App Service

- `AZURE_APP_SERVICE_NAME`
  - App Service name, e.g. `satlink-api-prod`.

## 3. Pipeline description

## 3.1. CI (`satlink-api-ci.yml`)

Triggers:
- Push to `main`.
- Pull Requests.

Steps:
1. Checkout.
2. Set up .NET 10.
3. SonarScanner `begin` (si hay secrets).
4. Restore.
5. Build.
6. Test with coverage (`XPlat Code Coverage`).
7. SonarScanner `end` (si hay secrets).
8. Publish `TestResults` artifacts.

## 3.2. CD (`satlink-api-cd.yml`)

Trigger:
- Push to `main`.

Recommended protection:
- The CD workflow uses a **GitHub Environment** named `production`.
- Configure approvals in: **Repository Settings → Environments → production → Required reviewers**.
- With this, each CD run will be **paused** until approved.

Steps:
1. Checkout.
2. Azure login (OIDC).
3. Login ACR.
4. Build the Docker image using `Satlink.Api/Dockerfile`.
5. Push to ACR with tag equal to `github.sha`.
6. Deploy to Azure App Service with `azure/webapps-deploy@v3`.

## 4. Azure application settings

Recommendation:
- Do not store secrets in `appsettings.json` in production.
- Configure `Jwt:Key` and connection strings via App Service settings.

Suggested keys in App Service → Configuration → Application settings:
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__Key`
- `ConnectionStrings__SatlinkApp`

## 5. Troubleshooting

### 5.1. The workflow fails during restore
- Verify connectivity from GitHub runners to NuGet.
- Ensure you don't depend on private feeds without credentials.

### 5.2. SonarQube does not run
- Check that `SONAR_HOST_URL`, `SONAR_TOKEN` and `SONAR_PROJECT_KEY` are configured.
- The workflow skips SonarQube if any are missing.

### 5.3. CD is waiting for approval
- This is expected when the `production` environment has required reviewers.
- Go to the Actions tab and approve the deployment.

### 5.4. CD fails to log in to Azure
- Verify the Service Principal's federated credentials.
- Verify the SP permissions over ACR and App Service:
  - `AcrPush` on the ACR.
  - Contributor (or the minimum equivalent role) on the App Service.

### 5.5. App Service does not start the container
- Verify it's configured as a Linux container.
- Check container logs (App Service → Log stream).
- Verify `ACR_LOGIN_SERVER/ACR_REPOSITORY:tag`.

