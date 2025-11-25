# BlueChallenge

Projeto composto por uma API ASP.NET Core (net10.0) e um frontend Vue 3 + Vite. O objetivo é disponibilizar um fluxo simples de cadastro/login, criação de agendamentos e telemetria baseada em eventos com SQLite embarcado e RabbitMQ opcional.

## Estrutura

```
BlueChallenge.Api/        # API ASP.NET Core
BlueChallenge.Web/        # Frontend Vue 3
planejamento.md           # Documento de arquitetura/domínio
requisitos.txt / desafio.txt
```

## Pré-requisitos

- .NET SDK 10.0 preview (matching `net10.0` do projeto)
- Node.js 20 LTS + npm 10
- (Opcional) Docker/Docker Compose para empacotar API, frontend e RabbitMQ
- (Opcional) Instância RabbitMQ local ou em container

## Configuração da API (`BlueChallenge.Api`)

### App settings e credenciais

- O arquivo `appsettings.json` já aponta para `Data/app.db` (SQLite) e inclui os campos `User=blue` e `Password=challenge` para manter os mesmos dados caso a solução migre para outro provider.
- Para RabbitMQ não comite credenciais reais. Use variáveis de ambiente ou `dotnet user-secrets`:
  - `RabbitMq__User`
  - `RabbitMq__Password`
  - `RabbitMq__Host`, `RabbitMq__Port`, etc., se precisar customizar.
- O avaliador pode definir as próprias credenciais exportando as variáveis antes de executar `dotnet run`.

### Criar o banco SQLite

```pwsh
cd BlueChallenge.Api
mkdir Data -Force
# Crie a primeira migration (se ainda não existir)
dotnet ef migrations add InitialCreate
# Materialize o schema
dotnet ef database update
```

O arquivo `Data/app.db` deve ser versionado/empacotado com o projeto para facilitar a avaliação. Caso rode em Docker, monte um volume para preservar esse arquivo.

### Executar a API

```pwsh
cd BlueChallenge.Api
 dotnet run
```

A API expõe o Swagger/OpenAPI em `https://localhost:5001/swagger` quando em desenvolvimento.

## Configuração do frontend (`BlueChallenge.Web`)

```pwsh
cd BlueChallenge.Web
npm install
npm run dev
```

- O Vite usa as variáveis definidas em `env.d.ts`/`tsconfig.*`. Adicione arquivos `.env` conforme o backend for expondo URLs públicas.
- Testes: `npm run test:unit` (Vitest) e `npm run lint` para ESLint.

## RabbitMQ

- Para desenvolvimento rápido, suba um container com a UI de gerenciamento:  
  `docker run -d --name blue-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management`
- Ajuste as credenciais via variáveis, conforme descrito acima.
- Eventos previstos: `UserRegistered`, `UserFirstLogin`, `UserLogin`, além de futuros `ScheduleCreated/Updated`.

## Docker (opcional)

> Ainda não há `docker-compose.yml`, mas o plano é:
>
> - API: imagem baseada em `mcr.microsoft.com/dotnet/aspnet:8.0` (ou 10 preview quando disponível), copiando o `publish`.
> - Frontend: build em `node:20` e publicação em `nginx:alpine`.
> - RabbitMQ: serviço `rabbitmq:3-management` com volume para persistência.

Documente comandos como `docker compose up --build` quando a orquestração estiver pronta.

## Próximos passos

1. Finalizar a modelagem das entidades/repos no backend e expor os endpoints.
2. Conectar o frontend aos endpoints (login, agendamentos) e criar fluxos de UX mínimos.
3. Automatizar scripts de seed/fixtures para o avaliador.
4. Publicar instruções específicas no README sempre que novos comandos/serviços forem adicionados.
