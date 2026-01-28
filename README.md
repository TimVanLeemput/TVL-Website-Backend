# TVL Website Backend — Voting Poll API

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-10-512BD4?logo=dotnet)](https://learn.microsoft.com/en-us/aspnet/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Railway](https://img.shields.io/badge/Deployed_on-Railway-0B0D0E?logo=railway)](https://railway.app/)

A production-grade REST API powering the live weekly polls and authentication on [timvanleemput.com](https://timvanleemput.com). Built with ASP.NET Core 10 and Clean Architecture as my first dedicated .NET backend project.

---

## Features

- **Authentication** — JWT access tokens (15 min) + refresh token rotation, Argon2 password hashing, email verification with 24h tokens and 30s resend cooldown, password reset with 1h tokens
- **Weekly poll system** — 52 ISO-week polls seeded from data; current poll auto-resolved by `ISOWeek.GetWeekOfYear(DateTime.UtcNow)`
- **Rate limiting** — 40 req/min per authenticated user or IP (fixed window, partitioned)
- **Global exception middleware** — maps domain exceptions to correct HTTP status codes (400/401/403/409/429)
- **Input validation** — FluentValidation for password strength and all request DTOs
- **Health check** — `/health` endpoint with DB connectivity check

---

## Architecture

Clean Architecture with 3 decoupled layers:

```
src/
├── VotingPoll.API/             # Controllers, middleware, Program.cs
├── VotingPoll.Core/            # Entities, interfaces, services, DTOs, exceptions
├── VotingPoll.Infrastructure/  # EF Core DbContext, repositories, migrations
└── VotingPoll.Tests/           # xUnit + Moq integration tests (WebApplicationFactory)
```

Dependencies flow inward: `API → Core ← Infrastructure`

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| ORM | Entity Framework Core + Npgsql |
| Database (local) | PostgreSQL 16 via Docker Compose |
| Database (prod) | NeonDB (managed PostgreSQL) |
| Auth | JWT Bearer + refresh tokens |
| Password hashing | Argon2 (Konscious.Security.Cryptography) |
| Email | Resend API |
| Secrets (local) | Azure Key Vault |
| Validation | FluentValidation |
| Testing | xUnit + Moq + WebApplicationFactory |
| Deployment | Railway |

---

## Local Development

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for PostgreSQL)
- Azure Key Vault access (or manually set secrets in `appsettings.Development.json`)

### 1. Start the database

```bash
docker compose -f Docker/docker-compose.yml up -d
```

### 2. Configure secrets

Either via Azure Key Vault (set `KeyVaultUrl` in your environment), or add to `src/VotingPoll.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQL-Local": "Host=localhost;Port=5432;Database=VotingPollDb;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "VotingPollAPI",
    "Audience": "VotingPollClient"
  },
  "Resend": {
    "ApiKey": "your-resend-key"
  }
}
```

### 3. Apply migrations and run

```bash
cd src/VotingPoll.API
dotnet ef database update --project ../VotingPoll.Infrastructure
dotnet run
```

Swagger UI available at `http://localhost:5000/swagger`.

---

## API Endpoints

### Authentication — `/api/auth`

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/register` | — | Register new user |
| POST | `/login` | — | Login, returns JWT + refresh token |
| POST | `/refresh` | — | Rotate refresh token |
| POST | `/logout` | ✓ | Revoke refresh token |
| GET | `/verify` | — | Verify email via token |
| POST | `/resend-verification` | — | Resend verification email (30s cooldown) |
| POST | `/forgot-password` | — | Send password reset email |
| POST | `/reset-password` | — | Reset password via token |

### Polls — `/api/polls`

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/` | — | Get all polls (filter: `?isOpen=true/false`) |
| GET | `/current` | — | Get current week's poll |
| GET | `/{id}` | — | Get poll by ID with options and vote counts |

### Votes — `/api/votes`

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/` | ✓ | Cast a vote on a poll option |
| GET | `/myvote/{pollId}` | ✓ | Get the authenticated user's vote for a poll |

---

## Running Tests

```bash
dotnet test src/VotingPoll.Tests
```

To see individual test names and results in the console:

```bash
dotnet test src/VotingPoll.Tests --logger "console;verbosity=detailed"
```

Tests use an in-memory database via `WebApplicationFactory` — no running database or external services required.

**Coverage:**
- **Unit tests** — `VotingService` business logic (poll not found, already voted, invalid option, closed poll, valid vote)
- **Unit tests** — `PollService` vote percentage calculation
- **Integration tests** — Poll API endpoints (GET, POST with validation cases)
- **Integration tests** — Auth API endpoints (register, login, forgot password, reset password)

---

## Deployment

Deployed on [Railway](https://railway.app/) with:
- **NeonDB** — managed PostgreSQL (connection string via Railway env vars)
- **Resend** — transactional email (API key via Railway env vars)
- **No Azure Key Vault in production** — secrets injected directly as Railway environment variables

---

## License

[MIT](LICENSE) © 2026 Tim Van Leemput
