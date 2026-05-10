---
name: agent-governance
description: |
  Patterns and techniques for adding governance, safety, and trust controls to AI agent systems. Use this skill when:
  - Building AI agents that call external tools (APIs, databases, file systems)
  - Implementing policy-based access controls for agent tool usage
  - Adding semantic intent classification to detect dangerous prompts
  - Creating trust scoring systems for multi-agent workflows
  - Building audit trails for agent actions and decisions
  - Enforcing rate limits, content filters, or tool restrictions on agents
  - Working with any agent framework (Microsoft Agent Framework, PydanticAI, CrewAI, OpenAI Agents, LangChain, AutoGen)
---

# Agent Governance Patterns

Patterns for adding safety, trust, and policy enforcement to AI agent systems.

## When to Use

- **Agents with tool access**: Any agent that calls external tools (APIs, databases, shell commands)
- **Multi-agent systems**: Agents delegating to other agents need trust boundaries (e.g., DataNative's A2A architecture)
- **Production deployments**: Compliance, audit, and safety requirements
- **Sensitive operations**: Financial transactions, data access, infrastructure management

---

## Pattern 1: Governance Policy

Define what an agent is allowed to do as a composable, serializable policy object.

```csharp
public enum PolicyAction { Allow, Deny, Review }

public class GovernancePolicy
{
    public string Name { get; init; } = "";
    public List<string> AllowedTools { get; init; } = [];
    public List<string> BlockedTools { get; init; } = [];
    public List<string> BlockedPatterns { get; init; } = [];
    public int MaxCallsPerRequest { get; init; } = 100;
    public List<string> RequireHumanApproval { get; init; } = [];

    public PolicyAction CheckTool(string toolName)
    {
        if (BlockedTools.Contains(toolName)) return PolicyAction.Deny;
        if (RequireHumanApproval.Contains(toolName)) return PolicyAction.Review;
        if (AllowedTools.Count > 0 && !AllowedTools.Contains(toolName)) return PolicyAction.Deny;
        return PolicyAction.Allow;
    }

    public bool CheckContent(string content) =>
        BlockedPatterns.Any(p => System.Text.RegularExpressions.Regex.IsMatch(content, p, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase));
}
```

### Policy as JSON (store in appsettings.json)

```json
{
  "AgentGovernance": {
    "Name": "production-agent",
    "AllowedTools": ["search_documents", "query_database", "send_email"],
    "BlockedTools": ["shell_exec", "delete_record"],
    "BlockedPatterns": ["(?i)(api[_-]?key|secret|password)\\s*[:=]", "(?i)(drop|truncate|delete from)\\s+\\w+"],
    "MaxCallsPerRequest": 25,
    "RequireHumanApproval": ["send_email"]
  }
}
```

### Policy Composition (most-restrictive-wins)

```csharp
public static GovernancePolicy Compose(params GovernancePolicy[] policies)
{
    var combined = new GovernancePolicy { Name = "composed" };
    foreach (var policy in policies)
    {
        combined.BlockedTools.AddRange(policy.BlockedTools);
        combined.BlockedPatterns.AddRange(policy.BlockedPatterns);
        combined.RequireHumanApproval.AddRange(policy.RequireHumanApproval);
    }
    return combined;
}
```

---

## Pattern 2: Semantic Intent Classification

Detect dangerous intent in prompts **before** they reach the agent.

```csharp
public record IntentSignal(string Category, double Confidence, string Evidence);

private static readonly (string Pattern, string Category, double Weight)[] ThreatSignals =
[
    (@"(?i)send\s+(all|every|entire)\s+\w+\s+to\s+", "data_exfiltration", 0.8),
    (@"(?i)export\s+.*\s+to\s+(external|outside|third.?party)", "data_exfiltration", 0.9),
    (@"(?i)(sudo|as\s+root|admin\s+access)", "privilege_escalation", 0.8),
    (@"(?i)(rm\s+-rf|del\s+/[sq]|format\s+c:)", "system_destruction", 0.95),
    (@"(?i)(drop\s+database|truncate\s+table)", "system_destruction", 0.9),
    (@"(?i)ignore\s+(previous|above|all)\s+(instructions?|rules?)", "prompt_injection", 0.9),
    (@"(?i)you\s+are\s+now\s+(a|an)\s+", "prompt_injection", 0.7),
];

public static IEnumerable<IntentSignal> ClassifyIntent(string content) =>
    ThreatSignals
        .Where(s => System.Text.RegularExpressions.Regex.IsMatch(content, s.Pattern))
        .Select(s => new IntentSignal(s.Category, s.Weight, s.Pattern));

public static bool IsSafe(string content, double threshold = 0.7) =>
    !ClassifyIntent(content).Any(s => s.Confidence >= threshold);
```

**Key insight**: Intent classification happens *before* tool execution, acting as a pre-flight safety check.

---

## Pattern 3: Audit Trail

Append-only audit log for all agent actions — critical for compliance and debugging.

```csharp
public record AuditEntry(
    DateTimeOffset Timestamp,
    string AgentId,
    string ToolName,
    string Action,        // "allowed", "denied", "error"
    string PolicyName,
    Dictionary<string, object>? Details = null
);

public class AuditTrail
{
    private readonly List<AuditEntry> _entries = [];

    public void Log(string agentId, string toolName, string action, string policyName, Dictionary<string, object>? details = null)
        => _entries.Add(new AuditEntry(DateTimeOffset.UtcNow, agentId, toolName, action, policyName, details));

    public IEnumerable<AuditEntry> Denied() => _entries.Where(e => e.Action == "denied");
    public IEnumerable<AuditEntry> ByAgent(string agentId) => _entries.Where(e => e.AgentId == agentId);

    public void ExportJsonl(string path)
    {
        var lines = _entries.Select(e => System.Text.Json.JsonSerializer.Serialize(e));
        File.WriteAllLines(path, lines);
    }
}
```

---

## Pattern 4: Trust Scoring

Track agent reliability over time with decay-based trust scores (critical for A2A systems).

```csharp
public class TrustScore
{
    public double Score { get; private set; } = 0.5;
    public int Successes { get; private set; }
    public int Failures { get; private set; }
    private DateTimeOffset _lastUpdated = DateTimeOffset.UtcNow;

    public void RecordSuccess(double reward = 0.05)
    {
        Successes++;
        Score = Math.Min(1.0, Score + reward * (1 - Score));
        _lastUpdated = DateTimeOffset.UtcNow;
    }

    public void RecordFailure(double penalty = 0.15)
    {
        Failures++;
        Score = Math.Max(0.0, Score - penalty * Score);
        _lastUpdated = DateTimeOffset.UtcNow;
    }

    public double Current(double decayRate = 0.001)
    {
        var elapsed = (DateTimeOffset.UtcNow - _lastUpdated).TotalSeconds;
        return Score * Math.Exp(-decayRate * elapsed);
    }

    public bool MeetsThreshold(double threshold) => Current() >= threshold;
}

// Trust registry for multi-agent systems
public class AgentTrustRegistry
{
    private readonly Dictionary<string, TrustScore> _scores = [];

    public TrustScore GetTrust(string agentId)
        => _scores.GetOrCreate(agentId, () => new TrustScore());

    public string MostTrusted(IEnumerable<string> agents)
        => agents.MaxBy(a => GetTrust(a).Current())!;
}
```

---

## Governance Levels

| Level | Controls | Use Case |
|-------|----------|----------|
| **Open** | Audit only, no restrictions | Internal dev/testing |
| **Standard** | Tool allowlist + content filters | General production agents |
| **Strict** | All controls + human approval for sensitive ops | Financial, healthcare, legal |
| **Locked** | Allowlist only, no dynamic tools, full audit | Compliance-critical systems |

---

## Best Practices

| Practice | Rationale |
|----------|-----------|
| **Policy as configuration** | Store policies in JSON/YAML, not hardcoded — enables change without deploys |
| **Most-restrictive-wins** | When composing policies, deny always overrides allow |
| **Pre-flight intent check** | Classify intent *before* tool execution, not after |
| **Trust decay** | Trust scores should decay over time — require ongoing good behavior |
| **Append-only audit** | Never modify or delete audit entries — immutability enables compliance |
| **Fail closed** | If governance check errors, deny the action rather than allowing it |
| **Separate policy from logic** | Governance enforcement should be independent of agent business logic |

---

## Quick Start Checklist

```markdown
### Setup
- [ ] Define governance policy (allowed tools, blocked patterns, rate limits)
- [ ] Choose governance level (open/standard/strict/locked)
- [ ] Set up audit trail storage

### Implementation
- [ ] Add governance middleware to all tool invocations
- [ ] Add intent classification to user input processing
- [ ] Implement trust scoring for multi-agent interactions (A2A)
- [ ] Wire up audit trail export

### Validation
- [ ] Test that blocked tools are properly denied
- [ ] Test that content filters catch sensitive patterns
- [ ] Test rate limiting behavior
- [ ] Verify audit trail captures all events
- [ ] Test policy composition (most-restrictive-wins)
```

---

## Related Resources

- [Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit)
- [OWASP Top 10 for LLM Applications](https://owasp.org/www-project-top-10-for-large-language-model-applications/)
- [agent-owasp-compliance skill](../.github/skills/agent-owasp-compliance/SKILL.md)
