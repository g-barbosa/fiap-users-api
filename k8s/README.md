# Kubernetes Manifests - FIAP Users API

## Estrutura

```
k8s/
├── configmap.yaml      # Configurações não-sensíveis + PVC do SQL Server
├── secret.yaml         # Secrets (JWT, Connection String, SQL Server Password)
├── deployment.yaml     # Deployment da API + SQL Server
└── service.yaml        # Services (API + SQL Server)
```

## Pré-requisitos

- Cluster Kubernetes 1.25+ (Docker Desktop, Minikube, Kind, etc.)
- kubectl configurado
- Imagem Docker da API construída localmente

## Build da Imagem Docker

```bash
# Na raiz do projeto
docker build -t fiap-users-api:latest .
```

## Deploy

### Aplicar todos os recursos

```bash
# Aplicar na ordem correta
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/deployment.yaml

# Ou tudo de uma vez
kubectl apply -f k8s/
```

### Acessar localmente

```bash
# Port-forward para acessar a API
kubectl port-forward svc/users-api 8080:80

# Acessar em: http://localhost:8080
# Swagger: http://localhost:8080/swagger
```

### Verificar deploy

```bash
# Status dos pods
kubectl get pods

# Status dos deployments
kubectl get deployment

# Status dos services
kubectl get svc

# Logs da API
kubectl logs -l app=users-api -f

# Logs do SQL Server
kubectl logs -l app=sqlserver -f
```

## Componentes

### API (users-api)
- **Deployment**: 1 réplica, porta 8080
- **Service**: ClusterIP na porta 80
- **ConfigMap**: Variáveis de ambiente (ASPNETCORE_ENVIRONMENT, etc.)
- **Secret**: Connection string e configurações JWT

### SQL Server
- **Deployment**: SQL Server 2022, porta 1433
- **Service**: ClusterIP na porta 1433
- **PVC**: 1Gi de armazenamento persistente
- **Secret**: Senha do SA

## Configuração

### Secrets (IMPORTANTE!)

Antes de aplicar em produção, atualize os valores em `secret.yaml`:

```yaml
stringData:
  # API Secrets
  ConnectionStrings__DefaultConnection: "SUA_CONNECTION_STRING"
  Jwt__Key: "SUA_CHAVE_JWT_SEGURA"
  
  # SQL Server Secret
  MSSQL_SA_PASSWORD: "SUA_SENHA_SEGURA"
```

### Imagem Docker

Para produção, atualize a imagem no `deployment.yaml`:

```yaml
containers:
  - name: users-api
    image: seu-registry.azurecr.io/fiap-users-api:v1.0.0
    imagePullPolicy: Always
```

## Remover

```bash
kubectl delete -f k8s/
```
