# RankingUp — Backend

REST API and SignalR hub for the RankingUp sports ranking platform.

**License:** MIT — Copyright (c) 2026 Leonardo Dalben and Julian Nunes

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 (ASP.NET Core) |
| API | Minimal Hosting Model, Controllers |
| Real-time | SignalR (`/api/rankinghub`) |
| ORM | Dapper + Dapper.Contrib (raw SQL) |
| Mapping | AutoMapper 12 (MIT) |
| Messaging | MediatR 12 (MIT) |
| Database (active) | MySQL via MySqlConnector 2.5.0 (MIT) |
| Database (prepared) | PostgreSQL via Npgsql 10 (future migration) |
| API Docs | Swashbuckle / Swagger UI (`/swagger`) |
| Containerisation | Docker (multi-stage, built from repo root) |

---

## Architecture

Clean Architecture with vertical slices per domain aggregate:

```
Source/
├── RankingUp.WebApp.API          # Entry point — controllers, DI wiring, middleware
├── RankingUp.Core                # Cross-cutting — BaseEntity, BaseRepository (Dapper),
│                                 #   DbFactory, IDbStrategy, Settings, MediatR pipeline
├── RankingUp.Background.Service  # Hosted background services
│
├── RankingUp.Club.Domain         # Club aggregate — entities, interfaces
├── RankingUp.Club.Application    # Club use cases, AutoMapper profiles, MediatR handlers
├── RankingUp.Club.Data           # Club Dapper repositories
│
├── RankingUp.Player.Domain
├── RankingUp.Player.Application
├── RankingUp.Player.Data
│
├── RankingUp.Sport.Domain
├── RankingUp.Sport.Application
├── RankingUp.Sport.Data
│
├── RankingUp.Tournament.Domain
├── RankingUp.Tournament.Application  # Includes SignalR hub notifications
└── RankingUp.Tournament.Data

Test/
└── RankingUp.Club.Application.ViewModel.Tests
```

The data layer uses a **Strategy + Factory pattern** (`DbFactory` → `IDbStrategy`) to select the
database driver at runtime based on `appsettings.json DatabaseContext.DatabaseType`.

Soft-delete is implemented via an `IsDeleted` flag — `DeleteAsync<T>` intentionally calls
`CoreUpdateAsync` rather than a physical DELETE.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A running MySQL instance (or SQL Server — see configuration below)
- (Optional) Docker

---

## Running Locally

### 1. Configure secrets

The connection string is **not** committed. Set it via [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
cd Source/RankingUp.WebApp.API
dotnet user-secrets set "DatabaseContext:ConnectionString" "Server=localhost;Database=rankingup;User=root;Password=your_password;"
```

`DatabaseContext:DatabaseType` defaults to `1` (MySQL). Change to `2` for SQL Server.

### 2. Restore and run

```bash
dotnet restore
dotnet run --project Source/RankingUp.WebApp.API
```

The API will be available at:

- HTTP: `http://localhost:5216`
- HTTPS: `https://localhost:7056`
- Swagger UI: `https://localhost:7056/swagger`
- SignalR Hub: `https://localhost:7056/api/rankinghub`

CORS is configured to allow `http://localhost:5173` (Vite frontend dev server).

### 3. Run with Docker

The Dockerfile must be built from the **repository root** (not from inside the API folder):

```bash
docker build -f Source/RankingUp.WebApp.API/Dockerfile -t rankingup-back .
docker run -p 8080:80 \
  -e DatabaseContext__ConnectionString="Server=host.docker.internal;..." \
  rankingup-back
```

---

## Running Tests

```bash
dotnet test
```

---

## Open-Source Dependencies

All runtime dependencies are open-source with permissive licenses.

| Package | Version | License | Notes |
|---|---|---|---|
| AutoMapper | 12.0.1 | MIT | Pinned to 12.x — 13+ requires commercial license |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.0 | MIT | Pinned to 12.x |
| MediatR | 12.0.1 | Apache-2.0 | Pinned to 12.x — 13+ requires commercial license |
| Dapper.Contrib | 2.0.78 | Apache-2.0 | |
| MySqlConnector | 2.5.0 | MIT | Replaces GPL-2.0 `MySql.Data` |
| Npgsql | 10.0.2 | PostgreSQL License | Future PostgreSQL migration (inactive) |
| Microsoft.Data.SqlClient | 7.0.0 | MIT | Replaces deprecated/vulnerable `System.Data.SqlClient` |
| Swashbuckle.AspNetCore | 10.1.7 | MIT | |
| Microsoft.Extensions.Hosting.Abstractions | 10.0.0 | MIT | |

---

## Future: PostgreSQL Migration

The codebase is prepared for a future migration to PostgreSQL:

- `Npgsql` is already a dependency of `RankingUp.Core`
- `PostgreSqlStrategy` implements `IDbStrategy` and is registered in `DbFactory`
- `DatabaseType.PostgreSql = 3` exists in the `Settings` enum
- To activate: set `"DatabaseContext:DatabaseType": 3` and update the connection string
- When migrating, remove `MySqlGuidTypeConfig` (PostgreSQL has native UUID support and does not need the Dapper `Guid` type handler)
