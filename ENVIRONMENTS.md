VotingPoll API — Environment Overview
======================================

LOCAL DEVELOPMENT
- API runs via Docker Compose (containerized)
- Database: SQL Server running in Docker
- Secrets pulled from Azure Key Vault (JWT, connection string)
- DatabaseProvider = "SqlServer"
- Azure Key Vault is ONLY used locally

PRODUCTION (DEPLOYED)
- API container hosted on Railway (auto-deploys on push to main)
- Database: Neon (managed PostgreSQL)
- Secrets injected as Railway environment variables
- DatabaseProvider = "PostgreSQL"
- KeyVaultUrl is empty on Railway so Azure is skipped entirely

BASE URLS
- Local:      http://localhost:{PORT}
- Production: https://{railway-app-url}

AUTH FLOW
1. POST /api/auth/register  — create account
2. POST /api/auth/login     — returns access token + refresh token
3. Add header: Authorization: Bearer {access_token}
Access tokens expire after 15 min. Use POST /api/auth/refresh to renew.

HEALTH CHECK
GET /health
- No auth required
- Healthy = Railway container is up AND Neon DB is reachable
- Unhealthy = DB connection failed
