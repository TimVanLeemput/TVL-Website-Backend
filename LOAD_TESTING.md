# Load Testing Guide

A progressive set of load tests from gentle warm-up to full crash attempts.
Includes standard bombardier tests and attack simulations to bypass the rate limiter.

---

## Prerequisites

Make sure both are running before any test:

```bash
# Start Postgres
docker compose -f Docker/docker-compose.yml up -d

# Start the API (separate terminal)
dotnet run --project src/VotingPoll.API
```

Verify the stack is healthy:

```bash
curl http://localhost:5000/health
# Expected: Healthy
```

---

## Bombardier Flag Reference

| Flag | Description |
|------|-------------|
| `-c N` | Number of concurrent connections (requests in-flight at the same time) |
| `-n N` | Total number of requests to send, then stop |
| `-d Ns` | Run for N seconds instead of a fixed count |
| `--rate N` | Cap requests per second (without this, bombardier fires as fast as possible) |

---

## Standard Load Tests

### Tier 1 — Warm Up

Single connection, 20 requests. Just confirms the endpoint responds.

```bash
./bombardier-linux-amd64 -c 1 -n 20 http://localhost:5000/api/polls
```

- `-c 1` — one request at a time, no overlap
- `-n 20` — stops after 20 requests

---

### Tier 2 — Light Load

Simulates a handful of real users browsing at the same time.

```bash
./bombardier-linux-amd64 -c 10 -n 100 http://localhost:5000/api/polls
```

- `-c 10` — 10 requests in-flight simultaneously
- `-n 100` — 100 requests total

---

### Tier 3 — Moderate Load

A shared link getting some traffic. Rate limiter will start rejecting here.

```bash
./bombardier-linux-amd64 -c 50 -n 500 http://localhost:5000/api/polls
```

- `-c 50` — 50 concurrent connections
- `-n 500` — 500 requests total

---

### Tier 4 — Sustained Load

Steady traffic for 30 seconds. Tests stability over time, not just a burst.

```bash
./bombardier-linux-amd64 -c 100 -d 30s http://localhost:5000/api/polls
```

- `-c 100` — 100 concurrent connections
- `-d 30s` — keeps firing for 30 seconds regardless of how many requests that is

---

### Tier 5 — Heavy Load

Starts pushing the DB connection pool (default pool size is ~100).

```bash
./bombardier-linux-amd64 -c 200 -n 5000 http://localhost:5000/api/polls
```

- `-c 200` — exceeds the default DB pool — connections start queuing
- `-n 5000` — 5,000 requests total

---

### Tier 6 — Stress Test

Heavy sustained load for a full minute. Exposes memory leaks and slow degradation.

```bash
./bombardier-linux-amd64 -c 300 -d 60s http://localhost:5000/api/polls
```

- `-c 300` — well beyond default DB pool size
- `-d 60s` — long enough to see latency creep up over time

---

### Tier 7 — Spike Test

Simulates going viral — a sudden wall of traffic all at once.

```bash
./bombardier-linux-amd64 -c 500 -n 10000 http://localhost:5000/api/polls
```

- `-c 500` — 500 connections firing simultaneously
- `-n 10000` — 10,000 requests total

---

### Tier 8 — Extreme

Pushes OS socket limits, .NET thread pool, and memory simultaneously.

```bash
./bombardier-linux-amd64 -c 1000 -d 30s http://localhost:5000/api/polls
```

- `-c 1000` — 1,000 concurrent connections
- `-d 30s` — 30 seconds sustained — watch CPU and RAM spike

---

### Tier 9 — Nuclear

Max everything. Will crash an unprotected server.

```bash
./bombardier-linux-amd64 -c 2000 -d 60s --rate 10000 http://localhost:5000/api/polls
```

- `-c 2000` — 2,000 concurrent connections
- `-d 60s` — 60 seconds with no mercy
- `--rate 10000` — 10,000 requests per second — overwhelms rate limiter, DB pool, and OS socket limits

---

## Attack Simulations (Rate Limiter Bypass)

> **Note:** The rate limiter now partitions by `RemoteIpAddress` (fixed). The Host header attacks
> below are kept for reference and to test what happens if the fix is ever reverted. To re-enable
> the vulnerability for testing, temporarily swap back to `httpContext.Request.Headers.Host.ToString()`
> in `Program.cs`.

The original vulnerability partitioned by `Host` header, which is user-controlled and trivially
spoofed. Each unique `Host` value got its own bucket of 10 requests/min — meaning an attacker
could get unlimited requests through to the DB with a simple loop.

**Seed data before running these** — an empty DB won't stress Postgres:

```bash
docker exec -it TVL-Website-Backend-Postgres psql -U postgres -d VotingPollDb -c "
INSERT INTO \"Polls\" (\"Title\", \"CreatedAt\", \"ClosesAt\")
SELECT 'Poll ' || generate_series, NOW(), NOW() + INTERVAL '7 days'
FROM generate_series(1, 10000);"
```

### Bypass — Rotate Host Headers (100 fake hosts)

Each fake host gets its own rate limit window. 100 hosts = up to 1,000 requests reaching the DB.

```bash
for i in $(seq 1 100); do
  curl -s -o /dev/null -w "%{http_code}\n" \
    -H "Host: fake-host-$i.com" \
    http://localhost:5000/api/polls &
done
wait
```

---

### Bypass — Sustained Host Rotation (1000 hosts)

Simulates a distributed attack that fully bypasses the rate limiter.

```bash
for i in $(seq 1 1000); do
  curl -s -o /dev/null \
    -H "Host: attacker-$i.com" \
    http://localhost:5000/api/polls &
done
wait
```

---

### Bypass — Rapid Rotation Loop (continuous)

Keeps rotating hosts in a tight loop for 30 seconds. Generates continuous DB pressure.

```bash
end=$((SECONDS+30))
i=0
while [ $SECONDS -lt $end ]; do
  curl -s -o /dev/null \
    -H "Host: host-$i.com" \
    http://localhost:5000/api/polls &
  i=$((i+1))
done
wait
```

---

## Real Findings from Testing (05/03/2026)

These were observed during actual load testing sessions on this machine.

### Finding 1 — Rate Limiter Blocks Everything (expected)
Bombardier nuclear tier (2000c, 60s) produced **491 x 429, 9 x 200, 0 x 5xx**.
Postgres CPU stayed at **0%**. The rate limiter rejected everything before it touched the DB.
- Conclusion: rate limiter works correctly under pure bombardier load.

### Finding 2 — Host Header Bypass Reaches the DB
Rotating 2000 fake `Host` headers with curl bypassed the rate limiter entirely.
Postgres CPU spiked to **95%**, NET I/O hit **327MB**, BLOCK I/O hit **58.8MB**.
- Conclusion: `Host` header is user-controlled and must not be used as a partition key.
- Fix applied: switched partition key to `RemoteIpAddress`.

### Finding 3 — Connection Pool Exhaustion (full collapse)
Under sustained host-rotation attack with real data, Postgres hit the **100 connection limit**:
```
The connection pool has been exhausted, either raise 'Max Pool Size' (currently 100)
or 'Timeout' (currently 15 seconds) in your connection string.
```
New requests waited 15 seconds then failed with **500**. Server collapsed under load.
- Conclusion: default pool size of 100 is not enough under a real attack.
- Next defence: raise `Max Pool Size` in the connection string, or add IP-based rate limiting.

### Finding 4 — Empty DB Hides Problems
With no data in Postgres, even aggressive attacks showed **0% CPU** — queries returned instantly.
Always seed test data before stress testing or results are meaningless.

---

## What to Watch For

| Symptom | Likely Cause |
|---------|-------------|
| 4xx (429) dominating | Rate limiter working correctly |
| 5xx climbing | Server or DB crashing under load |
| Latency avg > 500ms | DB connection pool queuing |
| Latency avg > 2000ms | Server near collapse |
| Throughput dropping to 0 | Server fully unresponsive |
| `others` count rising | Connection refused — process likely killed |
| `/health` returning 503 | Server is in serious trouble |
| Postgres CPU 0% under load | DB is empty — seed data first |
| `connection pool exhausted` in logs | Too many concurrent queries — raise Max Pool Size |

---

## Monitor While Testing

Open extra terminals and run these alongside your load test:

**Terminal 2 — API health:**
```bash
watch -n 1 'curl -s http://localhost:5000/health'
```
If this starts failing or slowing down, the server is struggling.

**Terminal 3 — Docker/Postgres stats:**
```bash
watch -n 1 'docker stats --no-stream'
```

Or streaming (updates continuously):
```bash
docker stats
```

| Metric | What it means | Warning sign |
|--------|--------------|--------------|
| CPU % | How hard Postgres is working | Sustained 200%+ means it's overwhelmed |
| MEM USAGE | RAM used by the container | Only worry if it approaches your system limit |
| NET I/O | Data moving in/out of the container | High is expected under load |
| BLOCK I/O | Disk writes (WAL logs, data pages) | Very high means Postgres is flushing hard to disk |
| PIDs | Active worker processes | Climbing PIDs = Postgres spinning up more workers to cope |

---

## Reset After a Test

If the DB or server ends up in a broken state:

```bash
docker compose -f Docker/docker-compose.yml down -v
docker compose -f Docker/docker-compose.yml up -d
dotnet ef database update --project src/VotingPoll.Infrastructure --startup-project src/VotingPoll.API
```
