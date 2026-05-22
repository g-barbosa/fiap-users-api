# Kubernetes Manifests - FIAP Users API

## Estrutura

```
k8s/
├── configmap.yaml      # Configurações não-sensíveis
├── secret.yaml         # Secrets (JWT, Connection String)
├── deployment.yaml     # Deployment da API
└── service.yaml        # Service ClusterIP
```

## Pré-requisitos

- Cluster Kubernetes 1.25+ (Docker Desktop, Minikube, Kind, etc.)
- kubectl configurado

## Deploy

```bash
kubectl apply -f k8s/configmap.yaml -f k8s/secret.yaml -f k8s/deployment.yaml -f k8s/service.yaml
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

# Status do deployment
kubectl get deployment

# Logs da aplicação
kubectl logs -l app=users-api -f
```

## Configuração

### Secrets (IMPORTANTE!)

Antes de aplicar, atualize os valores em `secret.yaml`:

```yaml
stringData:
  ConnectionStrings__DefaultConnection: "SUA_CONNECTION_STRING"
  Jwt__Key: "SUA_CHAVE_JWT_SEGURA"
```

### Imagem Docker

Atualize a imagem no `deployment.yaml`:

```yaml
containers:
  - name: users-api
    image: seu-registry.azurecr.io/fiap-users-api:v1.0.0
```

## Remover

```bash
kubectl delete -f k8s/service.yaml -f k8s/deployment.yaml -f k8s/secret.yaml -f k8s/configmap.yaml
```
