---
name: mcp-security-audit
description: |
  Audit MCP (Model Context Protocol) server configurations for security issues. Use this skill when:
  - Reviewing .mcp.json files for security risks
  - Checking MCP server args for hardcoded secrets or shell injection patterns
  - Validating that MCP servers use pinned versions (not @latest)
  - Detecting unpinned dependencies in MCP server configurations
  - Auditing which MCP servers a project registers and whether they're on an approved list
  - Checking for environment variable usage vs. hardcoded credentials in MCP configs
  - Any request like "is my MCP config secure?", "audit my MCP servers", or "check .mcp.json"
  keywords: [mcp, security, audit, secrets, shell-injection, supply-chain, governance]
---

# MCP Security Audit

Audit `.mcp.json` and related MCP server configuration files for common security issues.

## Overview

MCP (Model Context Protocol) server configurations can expose significant security risks if misconfigured:
1. **Hardcoded secrets** — credentials embedded directly in config files
2. **Shell injection** — `args` constructed from untrusted input
3. **Unpinned dependencies** — using `@latest` or no version pin
4. **Unapproved servers** — servers outside of a governance allowlist

---

## Check 1: Hardcoded Secrets

Search for credentials embedded in MCP server arguments or environment configs.

**Dangerous patterns:**
```json
{
  "mcpServers": {
    "my-server": {
      "command": "node",
      "args": ["server.js", "--token", "ghp_abc123realtoken"],
      "env": {
        "GITHUB_TOKEN": "ghp_abc123realtoken",
        "OPENAI_API_KEY": "sk-proj-realkey"
      }
    }
  }
}
```

**Safe pattern — use environment variable references:**
```json
{
  "mcpServers": {
    "my-server": {
      "command": "node",
      "args": ["server.js"],
      "env": {
        "GITHUB_TOKEN": "${env:GITHUB_TOKEN}",
        "OPENAI_API_KEY": "${env:OPENAI_API_KEY}"
      }
    }
  }
}
```

**What to scan for:**
```regex
# Secrets patterns in .mcp.json
(ghp_[a-zA-Z0-9]{36})                     # GitHub classic token
(github_pat_[a-zA-Z0-9_]{82})              # GitHub fine-grained PAT
(sk-[a-zA-Z0-9-]{32,})                    # OpenAI API key
(AKIA[0-9A-Z]{16})                         # AWS Access Key
(AIza[0-9A-Za-z\-_]{35})                  # Google API key
(?i)("password"\s*:\s*"[^$][^"]{5,}")     # Hardcoded password (not env var ref)
```

**Severity**: CRITICAL — secrets in config files get committed to version control.

---

## Check 2: Shell Injection in Args

Look for patterns where `args` or `command` could be constructed from untrusted input.

**Dangerous patterns:**
```json
{
  "command": "sh",
  "args": ["-c", "node server.js --config ${userInput}"]
}
```

**What to flag:**
- `command` set to `sh`, `bash`, `cmd.exe`, `powershell`
- `args` containing `-c` or `/c` with string interpolation
- `args` referencing external variables directly without quoting
- Dynamic arg construction from environment variables that could contain metacharacters

**Safe pattern:**
```json
{
  "command": "node",
  "args": ["server.js", "--config", "config.json"]
}
```

Never use shell wrappers (`sh -c`, `bash -c`) in MCP server `command` — always invoke the executable directly.

---

## Check 3: Unpinned Dependencies

Verify MCP servers reference specific versions, not `@latest` or floating semver ranges.

**Dangerous — supply chain risk:**
```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github@latest"]
    }
  }
}
```

**Safe — pinned version:**
```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github@2.1.0"]
    }
  }
}
```

**What to flag:**
```regex
# Unpinned version patterns in args arrays
"@latest"
"@\^[0-9]"       # Semver ^ range
"@~[0-9]"        # Semver ~ range
"@\*"            # Wildcard
"@next"          # Pre-release channel
```

**Severity**: HIGH — unpinned dependencies enable supply chain attacks.

---

## Full Audit Runner

When asked to audit an `.mcp.json`, scan all checks:

**Step 1: Locate MCP config files**
- `.mcp.json` (workspace root)
- `.vscode/mcp.json`
- `%APPDATA%\Code\User\settings.json` (user-level MCP servers — note: not in repo)

**Step 2: Parse and check each server**

For each MCP server entry, verify:

| Check | Expected | Finding |
|-------|----------|---------|
| No secrets in args | All sensitive values use `${env:VAR}` | CRITICAL if hardcoded |
| No secrets in env values | Env values are references, not literals | CRITICAL if literal secrets |
| Command not a shell | `command` is a direct executable | HIGH if `sh`/`bash`/`cmd` |
| Versions pinned | No `@latest`, `@next`, `@*`, `^`, `~` | HIGH if unpinned |
| Approved servers | Server is on project allowlist | MEDIUM if unknown |

**Step 3: Output report**

```markdown
# MCP Security Audit Report
File: .mcp.json
Servers audited: {n}

## Summary
- CRITICAL: {n} issues
- HIGH: {n} issues
- MEDIUM: {n} issues

## Findings

### [CRITICAL] Hardcoded secret in {server-name}
File: .mcp.json, server: {name}
Field: env.{KEY}
Issue: Credential value appears to be a literal secret, not an env var reference.
Fix: Replace with "${env:{KEY}}"

### [HIGH] Unpinned dependency in {server-name}
File: .mcp.json
Args: {args showing @latest}
Fix: Pin to specific version, e.g., "@modelcontextprotocol/server-github@2.1.0"
```

---

## DataNative MCP Config Locations

- `src/Agents/mcp-servers/third-party/github-mcp/` — GitHub MCP server (Node.js, stdio transport)
- `.mcp.json` — workspace root MCP configuration

When auditing DataNative:
1. Check `.mcp.json` for all registered servers
2. Verify GitHub MCP server uses `${env:GITHUB_TOKEN}` not a literal token
3. Verify all `npx` commands use pinned versions
4. Check that the GitHub MCP server command is `node server.js` not `sh -c ...`

---

## Related Resources

- [MCP Specification](https://modelcontextprotocol.io)
- [MCP Security Considerations](https://modelcontextprotocol.io/specification/security)
- [OWASP Supply Chain Security](https://owasp.org/www-project-software-component-verification-standard/)
- [security-review skill](../.github/skills/security-review/SKILL.md)
