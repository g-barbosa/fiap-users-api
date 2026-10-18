# Kubernetes Manifests - FIAP Users API

## Estrutura

```
k8s/
├── configmap.yaml      # Configurações não-sensíveis
├── secret.yaml         # Secrets da API (JWT, connection string, RabbitMQ)
├── deployment.yaml     # Deployment da API (2 réplicas, rolling update)
├── service.yaml        # Service ClusterIP
└── kustomization.yaml  # Tag da imagem e registry privado
```

## Pré-requisitos

- Cluster Kubernetes 1.25+ (gerenciado ou local)
- kubectl configurado
- Infra já aplicada a partir de `fiap-orchestration` (`kubectl apply -k k8s/`), inclusive o SQL Server

## Build da Imagem Docker

```bash
# Na raiz do projeto
docker build -t fiap-users-api:0.1.0 .
```

## Deploy

### Aplicar todos os recursos

```bash
# Aplicar na ordem correta
kubectl apply -k k8s/
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
- **Deployment**: 2 réplicas, RollingUpdate com `maxUnavailable: 0`, porta 8080
- **Service**: ClusterIP na porta 80 (exposição externa só pelo Kong)
- **ConfigMap**: Variáveis de ambiente (ASPNETCORE_ENVIRONMENT, etc.)
- **Secret**: Connection string e configurações JWT

O SQL Server fica em `fiap-orchestration/k8s/sqlserver` (uma instância, ClusterIP `sqlserver:1433`).

## Configuração

### Secrets (IMPORTANTE!)

Antes de aplicar em produção, atualize os valores em `secret.yaml`:

```yaml
stringData:
  # API Secrets
  ConnectionStrings__DefaultConnection: "SUA_CONNECTION_STRING"
  Jwt__Key: "SUA_CHAVE_JWT_SEGURA"
  
```

### Imagem e registry

A tag padrão é `0.1.0`. No cluster gerenciado, altere `images.newName` em `kustomization.yaml` para o registry privado da nuvem. Em EKS, a role da node group precisa de permissão de pull no ECR. Em AKS, ACR ou GKE, crie o Secret `cloud-registry` e referencie em `imagePullSecrets` se o cluster não tiver identidade integrada.

## Remover

```bash
kubectl delete -k k8s/
```
