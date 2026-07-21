# FIAP Users API

API de gerenciamento de usuários do **FIAP Cloud Games** 

## 🎯 Finalidade

Esta API é responsável pelo gerenciamento de usuários da plataforma FIAP Cloud Games, oferecendo:

- **Autenticação**: Login com geração de tokens JWT
- **Cadastro de Usuários**: Registro de novos usuários
- **Gerenciamento de Perfis**: CRUD de usuários com diferentes perfis (Admin, Comum)
- **Integração via Mensageria**: Comunicação assíncrona via RabbitMQ

## 🛠️ Tecnologias

- **.NET 8** - Framework principal
- **ASP.NET Core** - API REST
- **Entity Framework Core** - ORM
- **SQL Server** - Banco de dados
- **JWT** - Autenticação
- **RabbitMQ** - Mensageria
- **Serilog** - Logging
- **Prometheus** - Métricas (`/metrics`) via prometheus-net
- **Docker/Kubernetes** - Containerização e orquestração

## 🚀 Como Executar

### Local (Docker Compose)

```bash
docker-compose up -d
```

### Kubernetes

```bash
# Build da imagem
docker build -t fiap-users-api:latest .

# Deploy no cluster
kubectl apply -f k8s/
```

### Desenvolvimento

```bash
cd src/FiapCloudGames.Users.API
dotnet run
```

## 🔧 Variáveis de Ambiente

### Configurações Gerais

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução | `Development` |
| `ASPNETCORE_URLS` | URL de bind da aplicação | `http://+:8080` |

### Banco de Dados

| Variável | Descrição |
|----------|-----------|
| `ConnectionStrings__DefaultConnection` | Connection string do SQL Server |

### JWT (Autenticação)

| Variável | Descrição |
|----------|-----------|
| `Jwt__Key` | Chave secreta para assinatura de tokens (mín. 32 caracteres) |
| `Jwt__Issuer` | Emissor do token JWT |
| `Jwt__Audience` | Audiência do token JWT |
| `Jwt__ExpiracaoMinutos` | Tempo de expiração do token em minutos |

### RabbitMQ (Mensageria)

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `RabbitMq__Host` | Host do RabbitMQ | `rabbitmq` |
| `RabbitMq__Port` | Porta do RabbitMQ | `5672` |
| `RabbitMq__Username` | Usuário do RabbitMQ | - |
| `RabbitMq__Password` | Senha do RabbitMQ | - |

### Logging

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `Logging__LogLevel__Default` | Nível de log padrão | `Information` |
| `Logging__LogLevel__Microsoft.AspNetCore` | Nível de log do ASP.NET | `Warning` |
| `Logging__LogLevel__Microsoft.EntityFrameworkCore` | Nível de log do EF Core | `Warning` |

## 📁 Estrutura do Projeto

```
├── src/
│   ├── FiapCloudGames.Users.API/          # API (Controllers, Middlewares)
│   ├── FiapCloudGames.Users.Application/  # Camada de Aplicação (Services, DTOs)
│   ├── FiapCloudGames.Users.Domain/       # Domínio (Entidades, Interfaces)
│   └── FiapCloudGames.Users.Infrastructure/ # Infraestrutura (EF, Repositories)
├── k8s/                                    # Manifestos Kubernetes
│   ├── configmap.yaml                      # Configurações não-sensíveis
│   ├── secret.yaml                         # Secrets (JWT, Connection String)
│   ├── deployment.yaml                     # Deployments (API + SQL Server)
│   └── service.yaml                        # Services
└── Dockerfile
```

## 🌐 Endpoints

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/usuarios` | Cadastrar usuário | ❌ |
| POST | `/api/usuarios/login` | Autenticar usuário | ❌ |
| GET | `/api/usuarios` | Listar usuários | ✅ Admin |
| GET | `/api/usuarios/{id}` | Buscar usuário por ID | ✅ |
| PUT | `/api/usuarios/{id}` | Atualizar usuário | ✅ |
| DELETE | `/api/usuarios/{id}` | Remover usuário | ✅ Admin |

**Swagger**: `http://localhost:8080/swagger`

## 🧪 Testes

```bash
# Testes unitários
dotnet test src/FiapCloudGames.Users.Tests

# Testes BDD
dotnet test src/FiapCloudGames.Users.BDD
```