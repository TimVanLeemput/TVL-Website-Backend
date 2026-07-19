# Training Sessions — Design (TVL-VR Chapter 8)

> Status: **DRAFT — awaiting Tim's approval.** Source: TVL-VR `Docs/MASTER-PLAN.md`
> Chapter 8 + `Docs/Chapters/07-analytics-local.md` (the `SessionResult` wire contract is
> fixed and consumed verbatim). Audited against this repo's existing patterns
> (Polls/Votes vertical) — everything below matches them unless explicitly called out.

---

## 1. Repo pattern audit (what this feature must match)

- **Layering:** `Core` (entities, DTOs, mappings, services, validators, custom
  exceptions, repo interfaces) → `Infrastructure` (AppDbContext, repositories,
  migrations) → `API` (thin controllers, middleware). Dependency direction inward.
- **Controllers** are thin: inject service + concrete FluentValidation validator,
  validate DTO explicitly, return `ActionResult<T>`; paging via `PagedList<T>` with
  optional `page`/`pageSize` query params (null = unpaged), `#region GET/POST` sections.
- **Services** own business logic behind `I*Service` interfaces in
  `Core/Interfaces/ServicesInterfaces`; repositories behind `I*Repository` in
  `Core/Interfaces/Repositories`; both registered scoped in `Program.cs` regions.
- **Mappings:** static extension classes in `Core/Mappings` (`ToEntity`/`ToDto`).
- **Errors:** domain exceptions in `Core/Exceptions/<Area>Exceptions/`, translated by
  `GlobalExceptionMiddleware`.
- **Auth:** JWT bearer (issuer/audience/lifetime/signature validated), `[Authorize]`
  per endpoint. Secrets via Azure Key Vault (prod) / appsettings (dev), Neon PostgreSQL
  via `DatabaseProvider` connection-string switch, migrations in Infrastructure.
- **Tests:** xUnit + `WebApplicationFactory<Program>` (`TestFactory`) with
  EnsureDeleted/EnsureCreated per test, plus plain service unit tests.

## 2. Wire contract (fixed, from Chapter 7)

`POST` body = the Unity `SessionResult` JSON, camelCase, exactly as `JsonUtility`
emits it: `schemaVersion`, `sessionId`, `procedureId`, `procedureTitle`,
`startedAtUtc`/`completedAtUtc` (ISO-8601 strings), `durationSeconds`, `score`,
`passed`, `passingScore`, `steps[]` (`index`, `instruction`, `durationSeconds`,
`completed`), `errors[]` (`stepIndex`, `ruleType`, `severity` (string),
`scoreDeduction`, `atSeconds`), `device` (`model`, `operatingSystem`, `appVersion`).

ASP.NET's default System.Text.Json camelCase handling matches this with ordinary
PascalCase DTO properties — no attributes needed. Timestamps arrive as strings and are
parsed to `DateTime` (UTC) during mapping, not by the serializer, so a malformed date is
a validation error (400), never a 500.

## 3. Entity model (relational children, not JSONB)

`Core/Entities/Training/`:

- **`TrainingSession`** — `Id` (int PK), `SessionId` (Guid, **unique index** — client
  generated, the idempotency key), `SchemaVersion`, `ProcedureId` (indexed),
  `ProcedureTitle`, `StartedAtUtc`, `CompletedAtUtc`, `DurationSeconds`, `Score`,
  `Passed`, `PassingScore`, flattened device columns (`DeviceModel`,
  `DeviceOperatingSystem`, `AppVersion`), `ReceivedAtUtc` (server-set), plus
  `List<TrainingSessionStep> Steps`, `List<TrainingSessionError> Errors`.
- **`TrainingSessionStep`** — `Id`, `TrainingSessionId` (FK, cascade delete), `Index`,
  `Instruction`, `DurationSeconds`, `Completed`.
- **`TrainingSessionError`** — `Id`, `TrainingSessionId` (FK, cascade delete),
  `StepIndex`, `RuleType`, `Severity` (string, as on the wire), `ScoreDeduction`,
  `AtSeconds`.

Why relational over a JSONB blob: matches every existing entity in this repo (no JSONB
precedent), and the Chapter 9 dashboard wants aggregates ("most-failed step",
error-type frequency) that are trivial over child tables and awkward over JSON columns.
Sessions are write-once (no update endpoints), so the extra tables cost nothing in
complexity.

Migration: `AddTrainingSessions` (one migration, three tables + indexes on
`SessionId` unique, `ProcedureId`, `StartedAtUtc`).

## 4. Endpoints

### `POST /api/training-sessions` — device upload
- **Auth: static device API key** in an `X-Device-Key` header (see §5).
- Body: the Chapter 7 contract → `CreateTrainingSessionDto` (mirrors the wire shape,
  strings for timestamps).
- FluentValidation (injected validator, controller-side, like `VotesController`); on
  failure → 400 with the `ValidationResult` (existing convention).
- **Idempotent:** if `sessionId` already exists, return **200** with the existing
  session's summary DTO instead of inserting a duplicate — this is what makes the Unity
  offline queue's retry-after-crash safe. New session → **201 Created** + summary DTO.
- Rate limiting: covered by the existing global fixed-window limiter. **Call-out:**
  Kestrel's global `MaxRequestBodySize` is 10 KB; a 9-step session with a handful of
  errors is ~2–4 KB so it fits, but the POST action gets an explicit
  `[RequestSizeLimit(64 * 1024)]` so a long procedure can never be truncated by the
  global cap.

### `GET /api/training-sessions` — dashboard list
- **Auth: JWT** (`[Authorize]`) — instructor dashboard only.
- Query: `procedureId?`, `fromUtc?`, `toUtc?`, `page?`, `pageSize?` (PagedList
  convention, null = unpaged), ordered `StartedAtUtc` descending.
- Returns `PagedList<TrainingSessionSummaryDto>` — scalars only, no steps/errors
  (keeps list payloads small for the dashboard table).

### `GET /api/training-sessions/{id}` — dashboard detail
- **Auth: JWT.** Returns `TrainingSessionDetailDto` (summary + steps + errors).
- Unknown id → `TrainingSessionNotFoundException` → 404 via middleware.

## 5. Auth decision: device API key for POST, JWT for GET

**Decision:** the headset authenticates with a static API key
(`TrainingApi:DeviceKey` — Key Vault secret in prod, appsettings.Development locally),
checked by a small action filter (`[RequireDeviceKey]`) comparing the `X-Device-Key`
header with `CryptographicOperations.FixedTimeEquals`. The dashboard GETs reuse the
existing JWT pipeline untouched.

Trade-offs considered:
- **JWT for the device** would require an in-headset login flow (email/password in VR),
  token refresh handling in Unity, and a user identity the data model doesn't need —
  sessions are anonymous training runs, not user accounts. Rejected for scope, not
  difficulty.
- **API key risks:** a leaked key lets anyone post fake sessions (it cannot read
  anything). Acceptable for a portfolio deployment; mitigations already in place:
  HTTPS only, global rate limiter, strict validation. Key rotation = change one secret.
  A per-device key table is the obvious upgrade path if this ever ships to a fleet, and
  the filter is the single place that would change.

## 6. Validation rules (`CreateTrainingSessionValidator`)

- `schemaVersion` equals 1 (the gate that lets v2 change shape safely).
- `sessionId` parses as a GUID; `procedureId` non-empty, ≤ 128 chars;
  `procedureTitle` ≤ 256.
- `startedAtUtc`/`completedAtUtc` parse as ISO-8601; completed ≥ started;
  `durationSeconds` ≥ 0 (tolerance-checked against the timestamp delta is *not*
  enforced — clock drift on device is real).
- `score` and `passingScore` in 0–100; `steps` non-empty; each step: `index` ≥ 0,
  `durationSeconds` ≥ 0; each error: `stepIndex` ≥ 0, `ruleType`/`severity` non-empty,
  `scoreDeduction` > 0, `atSeconds` ≥ 0.
- Severity is deliberately **not** an enum whitelist — new rule types/severities from
  future procedures must not bounce off the backend.

## 7. New files (mirroring the Polls vertical)

| Layer | Files |
|---|---|
| Core/Entities/Training | `TrainingSession.cs`, `TrainingSessionStep.cs`, `TrainingSessionError.cs` |
| Core/Models/DTOs | `TrainingSessionDto.cs` (Create/Summary/Detail + step/error DTOs) |
| Core/Mappings | `TrainingSessionMappings.cs` |
| Core/Validation/TrainingValidators | `CreateTrainingSessionValidator.cs` |
| Core/Exceptions/TrainingExceptions | `TrainingSessionNotFoundException.cs` |
| Core/Interfaces | `ITrainingSessionService.cs`, `ITrainingSessionRepository.cs` |
| Core/Services | `TrainingSessionService.cs` |
| Infrastructure | `TrainingSessionRepository.cs`, DbSets in `AppDbContext`, migration |
| API | `TrainingSessionsController.cs`, `Filters/RequireDeviceKeyAttribute.cs` |
| Tests | `TrainingSessionsApiTests.cs`, `TrainingSessionServiceTests.cs` |

`Program.cs` additions: repository + service + validator registrations (validators are
already assembly-scanned; only DI registrations for repo/service needed).

## 8. Test plan

Integration (`TrainingSessionsApiTests`, TestFactory pattern):
1. POST valid session with device key → 201, row + children in DB.
2. POST same `sessionId` twice → second returns 200, still one row (idempotency).
3. POST without / with wrong `X-Device-Key` → 401.
4. POST invalid (schemaVersion 2, completed < started, empty steps, bad GUID) → 400.
5. GET list without JWT → 401; with JWT → 200 (uses the existing test auth handler).
6. GET list filters by `procedureId` and date range; paging returns correct
   `TotalCount`/page slice; ordering newest-first.
7. GET detail by id → steps + errors populated; unknown id → 404.

Unit (`TrainingSessionServiceTests`): mapping fidelity wire-DTO → entity → detail DTO
(timestamps parsed UTC, children aligned), idempotent-create branch.

## 9. Unity side (TVL-VR, branch `chapter/08-backend`, after backend is live)

`TVL.Analytics` gains `ApiClient` (UniTask): `UploadPending()` on app launch and on
session completion — reads `SessionStore.LoadAll()`, POSTs each un-uploaded session
(base URL + device key from a `BackendConfig` ScriptableObject, kept out of git via a
local asset), retry-once per file, on success moves the JSON into an `Uploaded/`
subfolder (the store's rotation then only prunes uploaded files — pending sessions are
never deleted unsent). 401/400 responses log loudly and leave the file pending (a 400
means a contract bug, not a transient failure). EditMode tests mock the transport;
end-to-end verify is one Editor run → row visible in NeonDB.

## 10. Task list (atomic commits, backend repo)

| # | Commit |
|---|---|
| 0 | this design doc |
| 1 | entities + DbContext + migration |
| 2 | DTOs + mappings + validator |
| 3 | repository + service + exceptions + DI |
| 4 | controller + device-key filter |
| 5 | integration + unit tests |
| 6 | fixes from verification; deploy migration to Neon |

Then TVL-VR: `ApiClient` + config + upload queue + tests (own commit series on
`chapter/08-backend`), end-to-end verify Editor → NeonDB, on-device checklist for Tim.
