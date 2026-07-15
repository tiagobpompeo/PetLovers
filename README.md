# PetLovers 🐾

Sistema de cadastro e gestão de pets em uma única solution .NET, com API REST, portal web HTML e app mobile .NET MAUI (Android e iOS).

## Estrutura

```
PetLovers.slnx
├── src/
│   ├── PetLovers.Domain           # Entidades e regras de negócio (C#)
│   ├── PetLovers.Infrastructure   # EF Core — SQL Server / SQLite
│   ├── PetLovers.API              # ASP.NET Core Web API (REST + Swagger)
│   ├── PetLovers.Web              # Site HTML (portal do tutor)
│   └── PetLovers.Mobile           # .NET MAUI — Android e iOS
└── tests/
    └── PetLovers.UnitTests        # Testes xUnit
```

## Pré-requisitos

- .NET 10 SDK
- Workload MAUI: `dotnet workload install maui`
- SQL Server (opcional — por padrão usa SQLite local)

## Como rodar

### API + Site Web
```bash
dotnet run --project src/PetLovers.API --launch-profile http
```
- Portal web: http://localhost:5155
- Swagger: http://localhost:5155/swagger

O banco é criado e populado automaticamente (seed) na primeira execução.

### Banco de dados

Por padrão usa **SQLite** (`petlovers.db`). Para usar **SQL Server**, em
`src/PetLovers.API/appsettings.json` defina:

```json
"Database": { "UseSqlServer": true },
"ConnectionStrings": { "SqlServer": "Server=...;Database=PetLovers;..." }
```

### App Mobile (.NET MAUI)
```bash
# Android — emulador (o app usa http://10.0.2.2:5155 automaticamente)
dotnet build src/PetLovers.Mobile -f net10.0-android -t:Run -p:AdbTarget="-s emulator-5554"

# Android — dispositivo físico via USB (o app usa http://localhost:5155 pelo túnel adb)
adb -s <SERIAL> reverse tcp:5155 tcp:5155   # refazer a cada reconexão do USB
dotnet build src/PetLovers.Mobile -f net10.0-android -t:Run -p:AdbTarget="-s <SERIAL>"

# iOS (requer Mac + Xcode)
dotnet build src/PetLovers.Mobile -f net10.0-ios -t:Run
```
A detecção é automática (`DeviceInfo.DeviceType == Virtual` → emulador). Use `adb devices` para obter o serial. Com múltiplos dispositivos conectados, `-p:AdbTarget` escolhe o alvo.

### Testes
```bash
dotnet test tests/PetLovers.UnitTests
```

## API REST — principais endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/pets?busca=` | Lista pets (filtro por nome, raça, microchip, tutor) |
| GET | `/api/pets/{id}` | Detalhe do pet com vacinas |
| POST | `/api/pets` | Cadastra pet |
| PUT | `/api/pets/{id}` | Atualiza pet |
| DELETE | `/api/pets/{id}` | Remove pet |
| POST | `/api/pets/{id}/vacinas` | Registra vacina |
| GET/POST/PUT/DELETE | `/api/tutores` | CRUD de tutores |
| GET | `/api/dashboard` | Indicadores (pets, tutores, vacinas pendentes, aniversariantes) |

## Azure DevOps

O pipeline CI está em `azure-pipelines.yml` (build + testes + artefatos).
Para publicar no Azure DevOps Repos:

```bash
git init
git add .
git commit -m "MVP PetLovers"
git remote add origin https://dev.azure.com/SUA_ORG/SEU_PROJETO/_git/PetLovers
git push -u origin main
```

## Próximos passos (fora do MVP)

- Autenticação JWT / Identity
- Upload de fotos dos pets
- Agendamentos (consultas, banho/tosa) com lembretes
- Migrations do EF Core (hoje usa `EnsureCreated`)
- FluentValidation e Serilog
- Pipeline de publicação (CD) para Azure App Service e lojas mobile
