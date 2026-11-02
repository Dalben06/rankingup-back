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
| Database (local default) | PostgreSQL 17 via Npgsql 10 |
| Database (supported) | MySQL via MySqlConnector 2.5.0, SQL Server via Microsoft.Data.SqlClient |
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
└── RankingUp.Club.Application.Tests
    └── ViewModels
```

The data layer uses a **Strategy + Factory pattern** (`DbFactory` → `IDbStrategy`) to select the
database driver at runtime based on `appsettings.json DatabaseContext.DatabaseType`.

Soft-delete is implemented via an `IsDeleted` flag — `DeleteAsync<T>` intentionally calls
`CoreUpdateAsync` rather than a physical DELETE.

---

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) with Compose (recommended path)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) for local builds and tests

---

## Running Locally

### 1. Start the Docker stack (recommended)

The repository ships a PostgreSQL 17 + API Compose stack. It creates the schema, loads
reference/demo data on first start, and exposes the API on port `8080`.

```bash
cp .env.example .env
docker compose up --build --detach --wait
```

Verify it is ready:

```bash
curl http://localhost:8080/health/ready
```

Endpoints:

- API: `http://localhost:8080`
- Swagger UI: `http://localhost:8080/swagger`
- Health: `http://localhost:8080/health/live` and `http://localhost:8080/health/ready`
- SignalR Hub: `http://localhost:8080/api/rankinghub`
- PostgreSQL: `localhost:5432` (credentials from `.env`)

Demo data includes two sports, one club, and two players. Run the end-to-end smoke check:

```bash
./Scripts/smoke_local.sh
```

Reset the database and reseed from scratch (destroys local data):

```bash
docker compose down --volumes
docker compose up --build --detach --wait
```

The `docker compose` commands must run from the **repository root**; the API image is built with
the repo root as its Docker context.

### 2. Run against a database outside Docker

Set the connection string via [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
(never commit it):

```bash
cd Source/RankingUp.WebApp.API
dotnet user-secrets set "DatabaseContext:ConnectionString" "Host=localhost;Port=5432;Database=rankingup;Username=rankingup;Password=your_password"
```

`DatabaseContext:DatabaseType` values are `1` MySQL, `2` SQL Server, and `3` PostgreSQL. Local
development defaults to `3`; the schema and seed scripts are PostgreSQL-specific.

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

---

## Running Tests

```bash
dotnet test Test/RankingUp.Club.Application.Tests/RankingUp.Club.Application.Tests.csproj
```

The test project covers the Club application ViewModel validation rules. There is no configured
lint, formatter, or codegen step; `dotnet build` is the type-check.

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
| Npgsql | 10.0.2 | PostgreSQL License | Local/default provider |
| Microsoft.Data.SqlClient | 7.0.0 | MIT | Replaces deprecated/vulnerable `System.Data.SqlClient` |
| Swashbuckle.AspNetCore | 10.1.7 | MIT | |
| Microsoft.Extensions.Hosting.Abstractions | 10.0.0 | MIT | |

---

## Database Providers

`DbFactory` selects the driver from `DatabaseContext.DatabaseType`:

- `1` — MySQL
- `2` — SQL Server
- `3` — PostgreSQL (local default)

`Scripts/recreate_database.sql` and `Scripts/seed_local_database.sql` are PostgreSQL-specific.
PostgreSQL uses native UUIDs and `TIMESTAMPTZ`, so the Dapper `Guid` handler used for MySQL must
stay out of the PostgreSQL path.
