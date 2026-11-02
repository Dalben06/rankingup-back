# Repository Guide

## Toolchain and verification

- Use the .NET 10 SDK selected by `global.json` (`10.0.0`, `latestMinor` roll-forward).
- Run solution checks from the repository root in this order:

```bash
dotnet restore RankingUp.sln
dotnet build RankingUp.sln --no-restore
dotnet test RankingUp.sln --no-build --no-restore
```

- `dotnet build` is the compile/type-check step; this repository has no configured lint, formatter, codegen, CI, or active pre-commit check.
- Run the only test project with `dotnet test Test/RankingUp.Club.Application.Tests/RankingUp.Club.Application.Tests.csproj`.
- Focus MSTest runs with `--filter "FullyQualifiedName~<namespace-or-class>"`; the current test project is only an empty scaffold and has no production `ProjectReference`.

## Runtime configuration

- The API entrypoint is `Source/RankingUp.WebApp.API/Program.cs`; run it with `dotnet run --project Source/RankingUp.WebApp.API/RankingUp.WebApp.API.csproj`.
- Configure both `DatabaseContext:DatabaseType` and `DatabaseContext:ConnectionString` through User Secrets or environment variables, never tracked `appsettings*.json` files.
- `DatabaseType` values are `1` MySQL, `2` SQL Server, and `3` PostgreSQL. Local development and `compose.yaml` use PostgreSQL; `Scripts/recreate_database.sql` and `Scripts/seed_local_database.sql` are PostgreSQL-specific.
- `Scripts/recreate_database.sql` is PostgreSQL-specific and destructive; do not run it as a routine setup or verification step.
- Local launch profiles use `http://localhost:5216` and `https://localhost:7056`; Swagger is always enabled and SignalR is mapped at `/api/rankinghub`.
- CORS currently permits only `http://localhost:5173` with credentials.

## Architecture and wiring

- The aggregate slices are Club, Player, Sport, and Tournament; each is split into Domain, Application, and Data projects, with shared infrastructure in `RankingUp.Core`.
- The practical flow is API controller -> application service -> domain repository interface -> Data repository/raw Dapper SQL. Keep SQL and Dapper multi-mapping in Data projects.
- Register services and repositories in `Source/RankingUp.WebApp.API/Setup/DependencyInjection.cs`; register mappings through `Setup/AutoMapperConfig.cs`.
- Tournament domain events use an in-memory singleton queue polled by a hosted service, then MediatR handlers update persistence and publish SignalR messages. This queue is not durable across process restarts.

## Persistence conventions

- Entities generally use an integer `Id` plus public `UUId`; preserve the established spelling and plural entity/table names because mappings and external contracts rely on them.
- Dapper writes depend on attributes such as `[Table]`, `[Computed]`, `[Write(false)]`, `[OnlyInsert]`, and `[OnlyUpdate]`; inspect existing entities before adding persistence fields.
- Deletes are soft deletes: call the entity's `Disable(userId)` and then repository `DeleteAsync`. `BaseRepository.DeleteAsync` intentionally performs an update.
- Normal repository reads must explicitly retain `IsDeleted = FALSE`; there is no global query filter.
- Follow existing multi-write services by using `TransactionScopeAsyncFlowOption.Enabled`; omitting it breaks transactions across `await`.
- SQL uses PostgreSQL/MySQL-style constructs such as `FALSE` and `LIMIT/OFFSET`; compilation alone does not establish cross-provider compatibility.

## Compatibility and change scope

- Treat existing names such as `UUId`, `GetInicialData`, `Notificacoes`, `ItensPerPage`, `TornamentId`, and SignalR event `joinned` as compatibility-sensitive; do not silently correct them.
- User-facing validation and business messages are predominantly Portuguese; match the surrounding contract.
- Nullable settings, namespace style, and `DateTime.Now` versus `DateTime.UtcNow` are inconsistent across projects. Match the touched project and keep normalization out of unrelated changes.
- AutoMapper 12.x and MediatR 12.x are deliberately pinned; do not upgrade them incidentally.
- Build the API image from the repository root with `docker build -f Source/RankingUp.WebApp.API/Dockerfile -t rankingup-back .`; no canonical container port is configured, so verify runtime binding before documenting or changing port mappings.
