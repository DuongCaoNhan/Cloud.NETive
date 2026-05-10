---
name: security-review
description: 'AI-powered codebase security scanner that reasons about code like a security researcher — tracing data flows, understanding component interactions, and catching vulnerabilities that pattern-matching tools miss. Use this skill when asked to scan code for security vulnerabilities, find bugs, check for SQL injection, XSS, command injection, exposed API keys, hardcoded secrets, insecure dependencies, access control issues, or any request like "is my code secure?", "review for security issues", "audit this codebase", or "check for vulnerabilities". Covers injection flaws, authentication and access control bugs, secrets exposure, weak cryptography, insecure dependencies, and business logic issues across JavaScript, TypeScript, Python, Java, PHP, Go, Ruby, and Rust.'
---

# Security Review

Reason about code like a security researcher. Trace data flows, understand component interactions, and find vulnerabilities that pattern-matching tools miss.

---

## Execution Workflow

### Step 1 — Define scope

Identify what to scan:
- **Full codebase audit**: Scan all files, prioritize by risk surface
- **Feature audit**: Focus on new features or changed files
- **Targeted audit**: User specifies area (e.g., "check authentication", "review API endpoints")

### Step 2 — Dependency scan

Check for vulnerable packages before reviewing application code.

- For .NET: Look for packages with known CVEs using `dotnet list package --vulnerable`
- For npm/Node: Check `package.json` for known vulnerable versions
- For Python: Check `requirements.txt` / `pyproject.toml` against known advisories
- Flag packages with HIGH or CRITICAL CVEs immediately

```bash
# .NET vulnerability scan
dotnet list package --vulnerable --include-transitive

# npm audit
npm audit --audit-level=high
```

### Step 3 — Secrets and credentials scan

Scan for hardcoded secrets before logic analysis. Common patterns:

```regex
# API keys and tokens
(api[_-]?key|apikey)\s*[:=]\s*['"]?[a-zA-Z0-9_\-]{16,}
(secret|password|passwd|pwd)\s*[:=]\s*['"](?!{|\$)[^'"]{8,}

# Cloud provider patterns
(AKIA[0-9A-Z]{16})                    # AWS Access Key ID
(github_pat_[a-zA-Z0-9_]{82})         # GitHub PAT
(?i)ghp_[a-zA-Z0-9]{36}              # GitHub Classic Token
AIza[0-9A-Za-z\-_]{35}               # Google API Key
```

Severity: **CRITICAL** — any match requires immediate remediation.

### Step 4 — Deep vulnerability scan

Scan for OWASP Top 10 and common CWEs:

#### SQL Injection (CWE-89)

```csharp
// VULNERABLE: string concatenation in SQL
var cmd = new SqlCommand("SELECT * FROM Users WHERE id = " + userId);

// SAFE: parameterized query
var cmd = new SqlCommand("SELECT * FROM Users WHERE id = @id");
cmd.Parameters.AddWithValue("@id", userId);
```

#### Command Injection (CWE-78)

```csharp
// VULNERABLE: unsanitized input in shell command
Process.Start("cmd.exe", "/c " + userInput);

// SAFE: use programmatic API
var files = Directory.GetFiles(validatedPath, "*.txt");
```

#### XSS — Cross-Site Scripting (CWE-79)

```csharp
// VULNERABLE: raw HTML rendering
@Html.Raw(userInput)

// SAFE: encoded output (Blazor/Razor default)
@userInput
```

#### Path Traversal (CWE-22)

```csharp
// VULNERABLE: unrestricted file path
var content = File.ReadAllText("/uploads/" + filename);

// SAFE: validate and normalize path
var fullPath = Path.GetFullPath(Path.Combine(baseDir, filename));
if (!fullPath.StartsWith(baseDir)) throw new UnauthorizedAccessException();
var content = File.ReadAllText(fullPath);
```

#### Insecure Deserialization (CWE-502)

```csharp
// VULNERABLE: deserializing untrusted data with type information
var obj = JsonSerializer.Deserialize<object>(untrustedJson);
// Or worse: BinaryFormatter, XmlSerializer with polymorphism

// SAFE: deserialize to known type only
var order = JsonSerializer.Deserialize<OrderRequest>(untrustedJson);
```

#### SSRF — Server-Side Request Forgery (CWE-918)

```csharp
// VULNERABLE: user controls URL
var response = await httpClient.GetAsync(userProvidedUrl);

// SAFE: allowlist of permitted domains
var allowedHosts = new[] { "api.example.com", "data.example.com" };
var uri = new Uri(userProvidedUrl);
if (!allowedHosts.Contains(uri.Host)) throw new SecurityException("Host not allowed");
```

#### IDOR — Insecure Direct Object Reference (CWE-639)

```csharp
// VULNERABLE: no ownership check
var document = await db.Documents.FindAsync(documentId);

// SAFE: always scope to current user
var document = await db.Documents
    .Where(d => d.Id == documentId && d.OwnerId == currentUserId)
    .FirstOrDefaultAsync();
```

### Step 5 — Data flow analysis

Trace user input through the application:

1. **Entry points**: HTTP request bodies, query strings, headers, file uploads, form fields
2. **Sinks**: SQL queries, shell commands, file paths, HTML output, HTTP client calls
3. **Transformations**: Track how data is modified between entry and sink
4. **Missing validation**: Identify flows where input reaches sinks without sanitization

### Step 6 — Verification

For each vulnerability found:
- Confirm exploitability (is this reachable? what's the prerequisite?)
- Assess impact (data exposure, privilege escalation, availability)
- Check for existing mitigations (framework-level, middleware, WAF)
- Eliminate false positives before reporting

### Step 7 — Report

```markdown
# Security Review Report
Date: {date}
Project: DataNative

## Executive Summary
{X} vulnerabilities found: {critical} CRITICAL, {high} HIGH, {medium} MEDIUM, {low} LOW

## Findings

### [CRITICAL] {Vulnerability Type} in {File}:{Line}
**Severity**: CRITICAL
**CWE**: CWE-XXX
**Exploitability**: [How an attacker would exploit this]
**Impact**: [What happens if exploited]

**Vulnerable Code:**
\`\`\`csharp
// problematic code
\`\`\`

**Fixed Code:**
\`\`\`csharp
// corrected code
\`\`\`

**References**: [CVE/CWE/OWASP links]
```

### Step 8 — Patches

Generate ready-to-apply patches for all confirmed vulnerabilities. For each finding:
1. Show the exact vulnerable code
2. Show the fixed version with explanation
3. Note any related changes needed (e.g., migration for schema changes)

---

## Severity Guide

| Severity | Definition | SLA |
|----------|-----------|-----|
| **CRITICAL** | Remote code execution, auth bypass, data exfiltration, hardcoded secrets | Fix immediately |
| **HIGH** | Privilege escalation, SQLi, XSS, SSRF, IDOR | Fix within 24h |
| **MEDIUM** | Information disclosure, missing rate limits, weak crypto | Fix within 1 week |
| **LOW** | Verbose error messages, missing security headers, minor config issues | Fix next sprint |
| **INFO** | Best practice improvements, defense-in-depth suggestions | Backlog |

---

## Output Rules

- **No false positives**: If not exploitable, mark as informational and explain why
- **Actionable findings**: Every HIGH+ finding must include a patch
- **Context-aware**: Consider framework mitigations (e.g., Blazor auto-encodes, EF Core parameterizes)
- **Ordered by risk**: Present CRITICAL first, then HIGH, MEDIUM, LOW
- **DataNative context**: Check against OWASP standards required by project security policy

---

## Detailed References

For detailed patterns and examples by vulnerability class, see the [awesome-copilot security-review references](https://github.com/github/awesome-copilot/tree/main/skills/security-review/references).
