# Codex Collaboration Preferences

## Claude CLI Opt-In Mode

Claude-assisted work is **off by default**.

### Commands (chat triggers)
- `claude_on` -> Codex may use the local `claude` CLI (`claude -p`) as a secondary assistant for this session/task.
- `claude_off` -> Codex must stop using the `claude` CLI unless explicitly requested again.

### How Codex should use Claude when `claude_on` is active
- Use Claude selectively (only when it adds value), not for every step.
- Show exactly what prompt was sent to Claude.
- Show Claude's answer clearly.
- Then provide Codex's own judgment/decision and implementation.

### Output style for Claude-assisted steps
- Use ANSI orange labels when supported by the UI:
  - `Claude prompt` (orange)
  - `Claude answer` (orange variant)

### Safety / clarity
- Do not paste or request raw secrets in chat if avoidable.
- Prefer non-interactive `claude -p` calls for transparency and repeatability.

