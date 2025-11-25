# BlueChallenge Telemetry

Worker em .NET 9 responsável por consumir eventos de telemetria via RabbitMQ e
armazená-los em arquivos separados por usuário.

## Visão geral

- Conecta ao RabbitMQ usando as configurações definidas em `appsettings.json`.
- Escuta a fila `telemetry` e processa mensagens assincronamente.
- Entende o envelope `{ "eventType", "timestamp", "payload" }` emitido pela API
  e reage aos eventos `UserCreated`, `UserUpdated`, `UserDeleted`,
  `ScheduleCreated`, `ScheduleUpdated`, `ScheduleDeleted` e
  `ScheduleServiceError`.
- Cada mensagem é registrada em `telemetry_logs/<usuario>.log` (caminho
  configurável via `Telemetry:StoragePath`).
- Útil para monitorar registros de ações como cadastro, login e criação de
  agendamentos publicados pela API principal.

## Configuração

Exemplo de `appsettings.json`:

```json
{
  "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "User": "guest",
    "Password": "guest"
  },
  "Telemetry": {
    "StoragePath": "C:/logs/telemetry"
  }
}
```

Se `Telemetry:StoragePath` não for informado, os arquivos serão criados em
`<pasta do worker>/telemetry_logs`.

## Execução local

```pwsh
cd BlueChallenge.Telemetry
 dotnet run
```

Certifique-se de que o RabbitMQ esteja rodando e que a fila `telemetry` exista
(como é declarada automaticamente, basta garantir o broker ativo).

## Estrutura principal

- `Program.cs`: registra o `Worker` padrão e o `RabbitMqConsumer`.
- `RabbitMqConsumer`: cria conexão/canal, consome mensagens e delega ao
  `TelemetryFileWriter`.
- `TelemetryFileWriter`: cria/gera arquivos por usuário com timestamp UTC.

## Próximos passos sugeridos

1. Persistir telemetria em um datastore consultável (SQLite/PostgreSQL ou
   serviço externo) para enriquecer dashboards.
2. Adicionar métricas/alertas (Prometheus, OTEL, etc.) para acompanhar volume e
   erros.
3. Empacotar como container e orquestrar via Docker Compose/Kubernetes junto com
   a API principal.
