# Satlink.Api - Deployment / CI-CD

Este documento describe cómo configurar el **CI/CD** de `Satlink.Api` con GitHub Actions, incluyendo:
- CI: build + tests + cobertura + SonarQube.
- CD: build Docker + push a ACR + deploy a Azure App Service (container) en `main`.

> Workflows:
> - `.github/workflows/satlink-api-ci.yml`
> - `.github/workflows/satlink-api-cd.yml`

## 1. Requisitos

- Repositorio en GitHub.
- Azure Subscription.
- Azure Container Registry (ACR).
- Azure App Service (Linux) configurado para ejecutar contenedor.
- GitHub Actions habilitado.

## 2. Secretos requeridos (GitHub)

Configurar en: **Repository Settings → Secrets and variables → Actions**.

### 2.1. SonarQube (CI)

El análisis se ejecuta solo si los secrets existen y no están vacíos.

- `SONAR_HOST_URL`
  - Ejemplo: `https://sonarqube.company.com`
- `SONAR_TOKEN`
  - Token de acceso generado en SonarQube.
- `SONAR_PROJECT_KEY`
  - Clave del proyecto en SonarQube.

### 2.2. Azure (CD)

Se usa login con OIDC (`azure/login@v2`).

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

> Estos valores provienen de una App Registration / Service Principal con federated credentials para GitHub.

### 2.3. Azure Container Registry (ACR)

- `ACR_NAME`
  - Nombre del ACR (sin FQDN), p.ej. `satlinkacr`.
- `ACR_LOGIN_SERVER`
  - Login server, p.ej. `satlinkacr.azurecr.io`.
- `ACR_REPOSITORY`
  - Repositorio de imagen dentro del ACR, p.ej. `satlink-api`.

### 2.4. Azure App Service

- `AZURE_APP_SERVICE_NAME`
  - Nombre del App Service, p.ej. `satlink-api-prod`.

## 3. Descripción de pipelines

## 3.1. CI (`satlink-api-ci.yml`)

Triggers:
- Push a `main`.
- Pull Requests.

Pasos:
1. Checkout.
2. Setup .NET 10.
3. SonarScanner `begin` (si hay secrets).
4. Restore.
5. Build.
6. Test con cobertura (`XPlat Code Coverage`).
7. SonarScanner `end` (si hay secrets).
8. Publicación de artifacts `TestResults`.

## 3.2. CD (`satlink-api-cd.yml`)

Trigger:
- Push a `main`.

Protección recomendada:
- El workflow de CD usa un **GitHub Environment** llamado `production`.
- Configura approvals en: **Repository Settings → Environments → production → Required reviewers**.
- Con esto, cada ejecución de CD quedará **pausada** hasta ser aprobada.

Pasos:
1. Checkout.
2. Login Azure (OIDC).
3. Login ACR.
4. Build Docker image usando `Satlink.Api/Dockerfile`.
5. Push a ACR con tag igual a `github.sha`.
6. Deploy a Azure App Service con `azure/webapps-deploy@v3`.

## 4. Variables de aplicación en Azure

Recomendación:
- No almacenar secretos en `appsettings.json` en producción.
- Configurar `Jwt:Key` y connection strings con App Settings del App Service.

Claves sugeridas en App Service → Configuration → Application settings:
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__Key`
- `ConnectionStrings__SatlinkApp`

## 5. Troubleshooting

### 5.1. El workflow falla en restore
- Verifica conectividad desde GitHub runners a NuGet.
- Asegura que no dependes de feeds privados sin credenciales.

### 5.2. SonarQube no se ejecuta
- Revisa que `SONAR_HOST_URL`, `SONAR_TOKEN` y `SONAR_PROJECT_KEY` estén configurados.
- El workflow omite SonarQube si falta alguno.

### 5.3. CD queda esperando aprobación
- Es el comportamiento esperado cuando el environment `production` tiene required reviewers.
- Revisa la pestaña Actions y aprueba el deployment.

### 5.4. CD falla al hacer login a Azure
- Verifica federated credentials del Service Principal.
- Verifica permisos del SP sobre ACR y App Service:
  - `AcrPush` sobre el ACR.
  - Contributor (o rol mínimo equivalente) para el App Service.

### 5.5. App Service no arranca el contenedor
- Verifica que está configurado como Linux container.
- Revisa logs de contenedor (App Service → Log stream).
- Verifica `ACR_LOGIN_SERVER/ACR_REPOSITORY:tag`.

