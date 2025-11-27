# BlueChallenge

Sistema de agendamentos com autenticação JWT, telemetria via RabbitMQ e interface web moderna construída com Vue 3 + PrimeVue.

## Índice

- [⚠️ Aviso Importante - Banco de Dados](#️-aviso-importante---banco-de-dados)
- [🔧 Configuração de Credenciais](#-configuração-de-credenciais)
- [Build com Docker](#build-com-docker)
- [Build e Execução Individual](#build-e-execução-individual)
- [Mapa de Funcionalidades](#mapa-de-funcionalidades)
- [Telemetria](#telemetria)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Integração RabbitMQ](#integração-rabbitmq)
- [Componentes do Frontend](#componentes-do-frontend)
- [Uso de IA no Desenvolvimento](#uso-de-ia-no-desenvolvimento)

---

## ⚠️ Aviso Importante - Banco de Dados

> **IMPORTANTE**: O arquivo `BlueChallenge.Api/storage/app.db` (SQLite) deve ser versionado/empacotado com o projeto para facilitar a avaliação. Este arquivo contém dados de exemplo para testes.

### Preservando o Banco no Docker

O `docker-compose.yml` já está configurado para montar o diretório `storage` como volume:

```yaml
volumes:
  - ./BlueChallenge.Api/storage:/data
```

Isso significa que:

- O arquivo `app.db` no seu projeto será usado pelo container
- Alterações feitas no banco serão persistidas no arquivo local
- Ao parar/reiniciar os containers, os dados serão preservados

### Resetando o Banco de Dados

Se precisar resetar o banco para o estado inicial:

```bash
# Pare os containers
docker-compose down

# Restaure o arquivo do git (se versionado)
git checkout BlueChallenge.Api/storage/app.db

# Reinicie os containers
docker-compose up -d
```

### Backup do Banco

```bash
# Copiar banco do container para máquina local
docker cp bluechallenge-api:/data/app.db ./backup-app.db

# Copiar banco local para o container (se necessário)
docker cp ./app.db bluechallenge-api:/data/app.db
```

---

## 🔧 Configuração de Credenciais

### Variáveis de Ambiente

O projeto utiliza as seguintes credenciais que podem ser configuradas:

| Variável                     | Padrão                            | Descrição                     |
| ---------------------------- | --------------------------------- | ----------------------------- |
| `Database__ConnectionString` | `Data Source=/data/app.db`        | String de conexão SQLite      |
| `RabbitMq__Host`             | `rabbitmq` (Docker) / `localhost` | Host do RabbitMQ              |
| `RabbitMq__Port`             | `5672`                            | Porta do RabbitMQ             |
| `RabbitMq__User`             | `guest`                           | Usuário RabbitMQ              |
| `RabbitMq__Password`         | `guest`                           | Senha RabbitMQ                |
| `RabbitMq__VirtualHost`      | `/`                               | Virtual host RabbitMQ         |
| `Jwt__Secret`                | (configurado em appsettings)      | Chave secreta para tokens JWT |
| `Jwt__Issuer`                | `BlueChallenge`                   | Emissor do token              |
| `Jwt__Audience`              | `BlueChallenge`                   | Audiência do token            |

### Configuração no Docker

Edite o `docker-compose.yml` para alterar credenciais:

```yaml
services:
  rabbitmq:
    environment:
      RABBITMQ_DEFAULT_USER: seu_usuario # Altere aqui
      RABBITMQ_DEFAULT_PASS: sua_senha # Altere aqui

  api:
    environment:
      RabbitMq__User: seu_usuario # Deve corresponder ao rabbitmq
      RabbitMq__Password: sua_senha # Deve corresponder ao rabbitmq
      Database__ConnectionString: "Data Source=/data/app.db"

  telemetry:
    environment:
      RabbitMq__User: seu_usuario # Deve corresponder ao rabbitmq
      RabbitMq__Password: sua_senha # Deve corresponder ao rabbitmq
```

### Configuração Local (sem Docker)

Edite os arquivos `appsettings.Development.json` em cada projeto:

**BlueChallenge.Api/appsettings.Development.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=storage/app.db"
  },
  "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "User": "guest",
    "Password": "guest"
  },
  "Jwt": {
    "Secret": "sua-chave-secreta-com-pelo-menos-32-caracteres",
    "Issuer": "BlueChallenge",
    "Audience": "BlueChallenge",
    "ExpirationMinutes": 60
  }
}
```

**BlueChallenge.Telemetry/appsettings.Development.json:**

```json
{
  "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "User": "guest",
    "Password": "guest",
    "TelemetryQueue": "telemetry"
  },
  "Telemetry": {
    "StoragePath": "telemetry_logs"
  }
}
```

**BlueChallenge.Web/.env.development:**

```env
VITE_API_URL=http://localhost:5000/api
```

---

## Build com Docker

A forma mais simples de executar todo o projeto é usando Docker Compose.

### Pré-requisitos

- Docker Desktop instalado e rodando

### Executar

```bash
# Na raiz do projeto
docker-compose up --build
```

### Serviços Disponíveis

| Serviço                 | URL                       | Descrição                             |
| ----------------------- | ------------------------- | ------------------------------------- |
| **Frontend**            | http://localhost:4173     | Interface web Vue 3                   |
| **API**                 | http://localhost:8080/api | API REST ASP.NET Core                 |
| **RabbitMQ Management** | http://localhost:15672    | Painel de gerenciamento (guest/guest) |

### Comandos Úteis

```bash
# Parar todos os containers
docker-compose down

# Rebuild apenas o frontend
docker-compose build web && docker-compose up -d web

# Rebuild apenas a API
docker-compose build api && docker-compose up -d api

# Ver logs de um serviço específico
docker-compose logs -f api
docker-compose logs -f web
docker-compose logs -f telemetry
```

---

## Build e Execução Individual

### Pré-requisitos

- .NET SDK 10.0 (preview)
- Node.js 20 LTS + npm
- RabbitMQ (opcional, para telemetria)

### API (BlueChallenge.Api)

```bash
cd BlueChallenge.Api

# Restaurar dependências
dotnet restore

# Criar/atualizar banco de dados SQLite
dotnet ef database update

# Executar em modo desenvolvimento
dotnet run
```

A API estará disponível em `http://localhost:5000` com Swagger em `/swagger`.

### Frontend (BlueChallenge.Web)

```bash
cd BlueChallenge.Web

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev

# Build para produção
npm run build

# Executar testes
npm run test:unit
```

O frontend estará disponível em `http://localhost:5173`.

### Serviço de Telemetria (BlueChallenge.Telemetry)

```bash
cd BlueChallenge.Telemetry

# Executar
dotnet run
```

---

## Mapa de Funcionalidades

### Controllers

| Controller            | Rota Base        | Descrição                |
| --------------------- | ---------------- | ------------------------ |
| `AuthController`      | `/api/auth`      | Autenticação de usuários |
| `UsersController`     | `/api/users`     | CRUD de usuários         |
| `SchedulesController` | `/api/schedules` | CRUD de agendamentos     |

#### AuthController

| Método | Rota              | Descrição                                 |
| ------ | ----------------- | ----------------------------------------- |
| POST   | `/api/auth/login` | Autenticação com email/senha, retorna JWT |

#### UsersController

| Método | Rota              | Descrição               |
| ------ | ----------------- | ----------------------- |
| GET    | `/api/users`      | Lista todos os usuários |
| GET    | `/api/users/{id}` | Busca usuário por ID    |
| POST   | `/api/users`      | Cria novo usuário       |
| PUT    | `/api/users/{id}` | Atualiza usuário        |
| DELETE | `/api/users/{id}` | Remove usuário          |

#### SchedulesController

| Método | Rota                  | Descrição                   |
| ------ | --------------------- | --------------------------- |
| GET    | `/api/schedules`      | Lista todos os agendamentos |
| GET    | `/api/schedules/{id}` | Busca agendamento por ID    |
| POST   | `/api/schedules`      | Cria novo agendamento       |
| PUT    | `/api/schedules/{id}` | Atualiza agendamento        |
| DELETE | `/api/schedules/{id}` | Remove agendamento          |

### Services

| Service                     | Responsabilidade                                           |
| --------------------------- | ---------------------------------------------------------- |
| `AuthenticationService`     | Validação de credenciais e geração de tokens JWT           |
| `UserService`               | Lógica de negócio para criação e validação de usuários     |
| `ScheduleService`           | Lógica de negócio para criação e validação de agendamentos |
| `JwtTokenService`           | Geração e validação de tokens JWT                          |
| `RabbitMqTelemetryProducer` | Envio de eventos de telemetria para o RabbitMQ             |

---

## Telemetria

O sistema de telemetria é um **worker isolado** (`BlueChallenge.Telemetry`) que roda separadamente da API. Essa arquitetura foi escolhida para:

- **Desacoplamento**: A API não precisa esperar a gravação dos logs
- **Escalabilidade**: O worker pode ser escalado independentemente
- **Resiliência**: Se o worker falhar, as mensagens ficam na fila do RabbitMQ
- **Performance**: A API responde mais rápido sem I/O de disco síncrono

A comunicação entre API e Telemetry é feita via **RabbitMQ** (mensageria), garantindo entrega confiável mesmo em caso de falhas.

### Arquitetura

```
┌─────────────┐     ┌──────────────┐     ┌─────────────────────┐
│   API       │────▶│   RabbitMQ   │────▶│ Telemetry Service   │
│ (Producer)  │     │   (Queue)    │     │ (Consumer + Writer) │
└─────────────┘     └──────────────┘     └─────────────────────┘
                                                   │
                                                   ▼
                                         ┌─────────────────┐
                                         │ Arquivos .log   │
                                         │ por usuário     │
                                         └─────────────────┘
```

### Verificando Telemetria com Docker

```bash
# Listar arquivos de telemetria
docker exec bluechallenge-telemetry ls -la /telemetry

# Ver conteúdo dos logs
docker exec bluechallenge-telemetry cat /telemetry/*.log

# Ver logs em tempo real do serviço
docker logs -f bluechallenge-telemetry
```

### Verificando Telemetria Localmente

Quando rodando localmente, os arquivos de telemetria são salvos em:

- **Windows**: `BlueChallenge.Telemetry/telemetry_logs/`
- **Ou no path configurado em** `appsettings.json` → `Telemetry:StoragePath`

```bash
# Listar arquivos
ls BlueChallenge.Telemetry/telemetry_logs/

# Ver conteúdo
cat BlueChallenge.Telemetry/telemetry_logs/*.log
```

### Formato dos Logs

Cada arquivo é nomeado pelo identificador do usuário (email) e contém linhas no formato:

```
[2025-11-26T23:45:00Z] {"event":"UserLogin","userId":"...","timestamp":"..."}
[2025-11-26T23:46:00Z] {"event":"ScheduleCreated","scheduleId":"...","userId":"..."}
```

### RabbitMQ Management

Acesse http://localhost:15672 (guest/guest) para:

- Visualizar filas e mensagens pendentes
- Monitorar taxa de mensagens
- Verificar conexões ativas

---

## Estrutura do Projeto

```
BlueChallenge/
├── BlueChallenge.Api/          # API ASP.NET Core
│   ├── controller/             # Controllers REST
│   │   ├── AuthController.cs
│   │   ├── UsersController.cs
│   │   └── SchedulesController.cs
│   ├── service/                # Lógica de negócio
│   │   ├── Auth/
│   │   │   ├── AuthenticationService.cs
│   │   │   ├── JwtTokenService.cs
│   │   │   └── ITokenService.cs
│   │   ├── telemetry/
│   │   │   ├── ITelemetryProducer.cs
│   │   │   └── RabbitMqTelemetryProducer.cs
│   │   ├── UserService.cs
│   │   └── ScheduleService.cs
│   ├── repository/             # Acesso a dados
│   │   ├── IUserRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── IScheduleRepository.cs
│   │   └── ScheduleRepository.cs
│   ├── model/                  # Entidades de domínio
│   │   ├── user/
│   │   │   ├── UserModel.cs
│   │   │   ├── EmailModel.cs
│   │   │   └── ...
│   │   ├── schedule/
│   │   │   └── ScheduleModel.cs
│   │   └── utils/
│   ├── Contracts/              # DTOs de Request/Response
│   │   ├── Auth/
│   │   ├── Users/
│   │   └── Schedules/
│   ├── validation/             # Validadores FluentValidation
│   ├── data/                   # DbContext e configurações EF
│   ├── configuration/          # Options pattern (JWT, RabbitMQ)
│   └── Migrations/             # Migrations do Entity Framework
│
├── BlueChallenge.Telemetry/    # Serviço consumidor de telemetria
│   ├── Worker.cs               # Background service principal
│   ├── RabbitMqConsumer.cs     # Consumidor da fila RabbitMQ
│   └── TelemetryFileWriter.cs  # Escritor de arquivos de log
│
├── BlueChallenge.Web/          # Frontend Vue 3
│   └── src/
│       ├── components/         # Componentes reutilizáveis
│       ├── views/              # Páginas/Views
│       ├── stores/             # Pinia stores
│       ├── services/           # Serviços de API
│       ├── types/              # Tipos TypeScript
│       └── router/             # Configuração de rotas
│
├── BlueChallenge.Api.Tests/    # Testes da API
│
└── docker-compose.yml          # Orquestração de containers
```

### Padrão de Arquitetura (API)

A API segue o padrão **Controller → Service → Repository**:

1. **Controller**: Recebe requisições HTTP, valida entrada e retorna respostas
2. **Service**: Contém lógica de negócio e orquestra operações
3. **Repository**: Abstrai acesso ao banco de dados via Entity Framework

### Models e Contracts

- **Models**: Entidades de domínio com lógica de negócio (ex: `UserModel`, `ScheduleModel`)
- **Contracts**: DTOs simples para serialização de requisições e respostas

---

## Integração RabbitMQ

### Produtor (API)

O `RabbitMqTelemetryProducer` na API publica mensagens na fila `telemetry`:

```csharp
public interface ITelemetryProducer
{
    Task PublishAsync(string userIdentifier, object payload, CancellationToken ct);
}
```

Eventos são publicados automaticamente pelos controllers após operações bem-sucedidas.

### Consumidor (Telemetry Service)

O `RabbitMqConsumer` é um `BackgroundService` que:

1. Conecta ao RabbitMQ
2. Consome mensagens da fila `telemetry`
3. Delega para `TelemetryFileWriter` que persiste em arquivos

### Configuração

Ambos os projetos usam a mesma estrutura de configuração:

```json
{
  "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "User": "guest",
    "Password": "guest",
    "TelemetryQueue": "telemetry"
  }
}
```

No Docker, o host é `rabbitmq` (nome do serviço).

---

## Componentes do Frontend

### Layout

| Componente       | Descrição                                             |
| ---------------- | ----------------------------------------------------- |
| `AppLayout.vue`  | Layout principal com sidebar e área de conteúdo       |
| `AppSidebar.vue` | Menu lateral de navegação com links e botão de logout |
| `AppHeader.vue`  | Cabeçalho com título da página e data atual           |

### Views (Páginas)

| View                | Rota         | Descrição                                        |
| ------------------- | ------------ | ------------------------------------------------ |
| `LoginView.vue`     | `/login`     | Tela de login com formulário de autenticação     |
| `RegisterView.vue`  | `/register`  | Tela de cadastro de novo usuário                 |
| `DashboardView.vue` | `/dashboard` | Dashboard com estatísticas e resumo              |
| `UsersView.vue`     | `/users`     | CRUD de usuários com DataTable                   |
| `SchedulesView.vue` | `/schedules` | CRUD de agendamentos com DataTable e formulários |

### Stores (Pinia)

| Store          | Responsabilidade                        |
| -------------- | --------------------------------------- |
| `auth.ts`      | Estado de autenticação, login/logout    |
| `users.ts`     | Estado e operações CRUD de usuários     |
| `schedules.ts` | Estado e operações CRUD de agendamentos |

### Services

| Service                | Responsabilidade                                   |
| ---------------------- | -------------------------------------------------- |
| `api.ts`               | Cliente HTTP base com interceptors de autenticação |
| `auth.service.ts`      | Login, logout, verificação de token                |
| `users.service.ts`     | Chamadas à API de usuários                         |
| `schedules.service.ts` | Chamadas à API de agendamentos                     |

### Tecnologias Frontend

- **Vue 3** com Composition API
- **TypeScript** para tipagem estática
- **PrimeVue** para componentes UI
- **Pinia** para gerenciamento de estado
- **Vue Router** para navegação
- **Vite** para build e dev server
- **Vitest** para testes unitários

---

## Uso de IA no Desenvolvimento

Este projeto utilizou assistência de IA (GitHub Copilot / Claude) nas seguintes áreas:

### Geração de Testes

- Testes unitários para stores Pinia (`auth.spec.ts`, `users.spec.ts`, `schedules.spec.ts`)
- Testes de serviços (`api.spec.ts`, `auth.service.spec.ts`, `users.service.spec.ts`, `schedules.service.spec.ts`)
- Mocks e fixtures para testes

### Documentação

- Este README foi gerado e estruturado com assistência de IA
- Documentação inline em código
- Comentários explicativos em configurações

### Snippets de Código

- Integração com RabbitMQ (producer/consumer)
- Configuração de telemetria e logging
- Componentes Vue com PrimeVue
- Configuração de nginx

### Boas Práticas

Todo código gerado por IA foi revisado e adaptado para manter consistência com o padrão do projeto e garantir funcionamento correto.

---

## Licença

Este projeto foi desenvolvido como parte de um desafio técnico.
