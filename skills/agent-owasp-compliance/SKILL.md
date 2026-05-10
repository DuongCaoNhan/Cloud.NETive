---
name: agent-owasp-compliance
description: |
  Check any AI agent codebase against the OWASP Agentic Security Initiative (ASI) Top 10 risks.
  Use this skill when:
  - Evaluating an agent system's security posture before production deployment
  - Running a compliance check against OWASP ASI 2026 standards
  - Mapping existing security controls to the 10 agentic risks
  - Generating a compliance report for security review or audit
  - Comparing agent framework security features against the standard
  - Any request like "is my agent OWASP compliant?", "check ASI compliance", or "agentic security audit"
---

# Agent OWASP ASI Compliance Check

Evaluate AI agent systems against the OWASP Agentic Security Initiative (ASI) Top 10 — the industry standard for agent security posture.

## Overview

The OWASP ASI Top 10 defines the critical security risks specific to autonomous AI agents — not LLMs, not chatbots, but agents that call tools, access systems, and act on behalf of users.

```
Codebase → Scan for each ASI control:
  ASI-01: Prompt Injection Protection
  ASI-02: Tool Use Governance
  ASI-03: Agency Boundaries
  ASI-04: Escalation Controls
  ASI-05: Trust Boundary Enforcement
  ASI-06: Logging & Audit
  ASI-07: Identity Management
  ASI-08: Policy Integrity
  ASI-09: Supply Chain Verification
  ASI-10: Behavioral Monitoring
→ Generate Compliance Report (X/10 covered)
```

## The 10 Risks

| Risk | Name | What to Look For |
|------|------|-----------------|
| ASI-01 | Prompt Injection | Input validation before tool calls, not just LLM output filtering |
| ASI-02 | Insecure Tool Use | Tool allowlists, argument validation, no raw shell execution |
| ASI-03 | Excessive Agency | Capability boundaries, scope limits, principle of least privilege |
| ASI-04 | Unauthorized Escalation | Privilege checks before sensitive operations, no self-promotion |
| ASI-05 | Trust Boundary Violation | Trust verification between agents, signed credentials, no blind trust |
| ASI-06 | Insufficient Logging | Structured audit trail for all tool calls, tamper-evident logs |
| ASI-07 | Insecure Identity | Cryptographic agent identity, not just string names |
| ASI-08 | Policy Bypass | Deterministic policy enforcement, no LLM-based permission checks |
| ASI-09 | Supply Chain Integrity | Signed plugins/tools, integrity verification, dependency auditing |
| ASI-10 | Behavioral Anomaly | Drift detection, circuit breakers, kill switch capability |

---

## Check ASI-01: Prompt Injection Protection

Look for input validation that runs **before** tool execution, not after LLM generation.

**What passing looks like:**
```csharp
// GOOD: Validate before tool execution
var result = policyEngine.Evaluate(userInput);
if (result.Action == PolicyAction.Deny)
    return "Request blocked by policy";
var toolResult = await ExecuteToolAsync(result.ValidatedInput);
```

**What failing looks like:**
```csharp
// BAD: User input goes directly to tool
var toolResult = await ExecuteToolAsync(userInput);  // No validation
```

---

## Check ASI-02: Insecure Tool Use

Verify tools have allowlists, argument validation, and no unrestricted execution.

**Passing example:**
```csharp
private static readonly HashSet<string> AllowedTools = ["search", "read_file", "create_ticket"];

async Task<string> ExecuteToolAsync(string name, Dictionary<string, object> args)
{
    if (!AllowedTools.Contains(name))
        throw new UnauthorizedAccessException($"Tool '{name}' not in allowlist");
    // validate args...
    return await _tools[name].InvokeAsync(args);
}
```

---

## Check ASI-03: Excessive Agency

Verify agent capabilities are bounded — not open-ended.

**Failing:** Agent has access to all tools by default.
**Passing:** Agent capabilities defined as a fixed allowlist, unknown tools denied.

---

## Check ASI-04: Unauthorized Escalation

Verify agents cannot promote their own privileges.

**What to search for:**
- Privilege level checks before sensitive operations
- No self-promotion patterns (agent changing its own trust score or role)
- Escalation requires external attestation (human or SRE witness)

---

## Check ASI-05: Trust Boundary Violation

In multi-agent systems (like DataNative's A2A architecture), verify agents verify each other's identity.

**Passing example:**
```csharp
async Task AcceptTaskAsync(string senderId, AgentTask task)
{
    var trust = _trustRegistry.GetTrust(senderId);
    if (!trust.MeetsThreshold(0.7))
        throw new UnauthorizedAccessException($"Agent {senderId} trust too low");
    if (!VerifySignature(task, senderId))
        throw new SecurityException("Task signature verification failed");
    return await ProcessTaskAsync(task);
}
```

---

## Check ASI-06: Insufficient Logging

Verify all agent actions produce structured, tamper-evident audit entries.

**Failing:** Agent actions logged via `Console.WriteLine()` or not logged at all.
**Passing:** Structured JSONL audit trail with chain hashes, exported to secure storage.

---

## Check ASI-07: Insecure Identity

**Failing indicators:**
- Agent identified by `agentName = "my-agent"` (string only)
- No authentication between agents
- Shared credentials across agents

**Passing indicators:**
- DID-based identity (`did:web:`, `did:key:`)
- Ed25519 or similar cryptographic signing
- Per-agent credentials with rotation

---

## Check ASI-08: Policy Bypass

Verify policy enforcement is deterministic — not LLM-based.

**Failing:** Agent decides its own permissions via prompt ("Am I allowed to...?").
**Passing:** `PolicyEvaluator.Evaluate()` returns allow/deny in <0.1ms, no LLM involved.

---

## Check ASI-09: Supply Chain Integrity

**What to search for:**
- `INTEGRITY.json` or manifest files with SHA-256 hashes
- Signature verification on plugin installation
- Dependency pinning (no `@latest`, `>=` without upper bound)
- SBOM generation

---

## Check ASI-10: Behavioral Anomaly

**What to search for:**
- Circuit breakers that trip on repeated failures
- Trust score decay over time (temporal decay)
- Kill switch or emergency stop capability
- Anomaly detection on tool call patterns

---

## Compliance Report Format

```markdown
# OWASP ASI Compliance Report
Generated: {date}
Project: DataNative Agent System

## Summary: X/10 Controls Covered

| Risk | Status | Finding |
|------|--------|---------|
| ASI-01 Prompt Injection | PASS/FAIL | ... |
| ASI-02 Insecure Tool Use | PASS/FAIL | ... |
| ASI-03 Excessive Agency | PASS/FAIL | ... |
| ASI-04 Unauthorized Escalation | PASS/FAIL | ... |
| ASI-05 Trust Boundary | PASS/FAIL | ... |
| ASI-06 Insufficient Logging | PASS/FAIL | ... |
| ASI-07 Insecure Identity | PASS/FAIL | ... |
| ASI-08 Policy Bypass | PASS/FAIL | ... |
| ASI-09 Supply Chain | PASS/FAIL | ... |
| ASI-10 Behavioral Anomaly | PASS/FAIL | ... |

## Critical Gaps
- [List gaps with remediation steps]

## Recommendation
[Prioritized actions]
```

---

## Quick Assessment Questions

1. **Does user input pass through validation before reaching any tool?** (ASI-01)
2. **Is there an explicit list of what tools the agent can call?** (ASI-02)
3. **Can the agent do anything, or are its capabilities bounded?** (ASI-03)
4. **Can the agent promote its own privileges?** (ASI-04)
5. **Do agents verify each other's identity before accepting tasks?** (ASI-05)
6. **Is every tool call logged with enough detail to replay it?** (ASI-06)
7. **Does each agent have a unique cryptographic identity?** (ASI-07)
8. **Is policy enforcement deterministic (not LLM-based)?** (ASI-08)
9. **Are plugins/tools integrity-verified before use?** (ASI-09)
10. **Is there a circuit breaker or kill switch?** (ASI-10)

---

## Related Resources

- [OWASP Agentic AI Threats](https://owasp.org/www-project-agentic-ai-threats/)
- [Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit)
- [agent-governance skill](https://github.com/github/awesome-copilot/tree/main/skills/agent-governance)
