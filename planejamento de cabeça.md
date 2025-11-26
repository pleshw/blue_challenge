# Como vou executar

ps: vou usar banco de dados relacional pra tudo pra poder fazer tudo em sqlite e simplificar o projeto, mas usaria um não relacional pra lidar com os schedules já que tem natureza mais dinâmica;

ps: não vou fazer microsserviços para manter o projeto simples, mas para uma aplicação real dividiria em três serviços diferentes, um para acesso/credenciais, um para agendamentos e outro para telemetria.

## Fluxo do cliente

- Cliente se cadastra no sistema usando email e senha
- Cliente acessa o sistema
- Cliente pode inserir agendamentos
- Cliente pode completar o cadastro através de modal - inserir UserPersonalInfo - e melhorar inserção de agendamentos

## Fluxo técnico

- Cliente cadastra no sistema
  - Publica a mensagem UserRegistered que vai adicionar os dados na telemetria via Logger
- Cliente acessa o sistema usando UserCredentials - JWT simples pra não atrasar muito o desenvolvimento, OAuth ia ser o ideal
  - Publica a mensagem UserLogin para adicionar login na telemetria e mandar email avisando novo login no dispositivo
- Usuario insere agendamento
  - Controller usa ScheduleService para criar o modelo do agendamento através do ScheduleBuilder
  - ScheduleService usa o ScheduleRepository para cadastrar Schedule no banco
  - ScheduleService retorna evento criado e Controller retorna OK com o novo evento.

# Esquema de entidades e módulos

## Logger - A planejar

## Schedule[record]

- DateRange
- IsAllDay
- HourRange
- User

Todos os campos com validadores válidos.
IsAllDay = false: implica em HourRange requerido.
Usuario requerido

### ScheduleBuilder

### ScheduleService

### DateRange[record]

- StartDate
- EndDate

### HourRange[record]

- StartHour
- EndHour

#### ScheduleValidator

#### DateRangeValidator

#### HourRangeValidator

## User[record]

- Id
- UserCredentials
- UserPersonalInfo

### UserService

### UserCredentials[record]

- Email
- Password
- User

### UserPersonalInfo[record]

- Name
- Phone
- User

### Phone[record]

- DDD
- Number

### Email[record]

- Alias
- Provider
- FullEmail

#### PasswordValidator

#### PhoneValidator

#### EmailValidator
