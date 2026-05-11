# ADR-001: Hybrid Service Architecture Patterns

- **Status:** Accepted
- **Date:** 2026-05-05
- **Deciders:** Architecture Review Board

## Context

The Cloud.NETive solution is a distributed microservices system. During architecture review, an apparent inconsistency was identified: some services use a strict 4-project Clean Architecture decomposition (e.g., AccountingService, AIService, InventoryService), while others use a single-project structure with internal folders (e.g., WebCrawler, TranslationApi). Additionally, some MCP servers are implemented in Node.js rather than .NET.

This ADR formally justifies and governs this intentional hybrid approach.

## Decision

We adopt a **tiered architecture model** where the project decomposition strategy is determined by service complexity and domain richness, not by uniformity.

### Tier 1 — Core Domain Services (4-Project Clean Architecture)

**Applies to:** AccountingService, AIService, InventoryService

**Structure:** `{Service}.API` / `{Service}.Application` / `{Service}.Domain` / `{Service}.Infrastructure`

**Rationale:**

- These services own complex business rules, aggregates, and multiple entities that require strict dependency inversion. The Domain project has zero external dependencies, enforced at compile time.
- Separate projects prevent accidental architectural coupling (e.g., Infrastructure leaking into Domain). A wrong `using` statement fails the build, not a code review.
- Domain and Application layers are independently unit-testable without instantiating any infrastructure concerns.
- Project-level separation supports team boundary alignment as the engineering team scales per domain.

### Tier 2 — Utility/Pipeline Services (Single-Project Vertical Slice)

**Applies to:** WebCrawler, TranslationApi

**Structure:** Single `.csproj` with internal folders (`Domain/`, `Infrastructure/`, `Consumers/`, `Saga/`, etc.)

**Rationale:**

- These services are heavily I/O-bound or integration-focused and have a minimal or entirely absent domain model. There are no complex aggregates or cross-entity invariants to protect.
- Decomposing a pipeline service into 4 projects ("Project Explosion") adds solution entries, build graph nodes, NuGet restore overhead, and folder navigation cost with zero architectural benefit.
- Incremental build time increases for a 4-project structure where no layer boundary provides meaningful isolation value.
- A single project with well-named internal folders (Vertical Slice Architecture) is easier to navigate and onboard for pipeline-style services.
- Architectural rules still apply within the project: Infrastructure code must not be consumed directly by `Consumers/` or `Saga/` code without passing through defined contracts in `Domain/` or `Core/`.

Example layout for `CloudNative.WebCrawler`:
```
CloudNative.WebCrawler/
├── Domain/          # Crawler entities, interfaces, value objects
├── Infrastructure/  # HTTP clients, storage adapters, parsers
├── Consumers/       # Message consumers (e.g., crawl-requested events)
└── Saga/            # Long-running crawl workflow coordination
```

### Tier 3 — Polyglot MCP Servers

**Applies to:** `src/Agents/mcp-servers/` (e.g., `cloudnative-tools/`)

**Rationale:**

- MCP (Model Context Protocol) servers are tool-provider processes that communicate over stdio or SSE transport. The protocol is language-agnostic by specification.
- The Node.js ecosystem provides superior, battle-tested MCP SDK support and a rich library of community-maintained server implementations (e.g., `@modelcontextprotocol/server-github`). Rewriting these in .NET sacrifices ecosystem maturity for superficial uniformity.
- These servers integrate into .NET Aspire orchestration as external executable resources, preserving lifecycle management, health checks, and structured log capture without requiring .NET as the runtime.

Example layout for `src/Agents/mcp-servers/`:
```
mcp-servers/
├── README.md
└── cloudnative-tools/          # Node.js MCP server example
    ├── package.json
    ├── index.ts
    └── tools/                  # Individual tool implementations
```

## Consequences

| Outcome | Details |
|---------|---------|
| **Consistency within tier** | All services within a tier must follow that tier's structural pattern. Any deviation from a tier's pattern requires a new ADR. |
| **Deliberate tier assignment** | Tier is assigned at service creation based on objective domain complexity criteria, not team preference or familiarity. |
| **Promotion path** | A Tier 2 service that grows a real domain model (3+ aggregates with invariants) must be formally promoted to Tier 1 via a tracked refactoring task and architectural review. |
| **Polyglot boundary** | Non-.NET runtimes are limited to the `mcp-servers/` subtree. All application services and HTTP APIs must be .NET. |
| **No retroactive uniformity** | Existing services will not be restructured purely for naming consistency. Restructuring is only triggered by a functional need or tier promotion. |

## Tier Assignment Criteria

| Criterion | Tier 1 (4-Project) | Tier 2 (Single-Project) |
|-----------|---------------------|--------------------------|
| Has 3+ domain aggregates with business invariants | Required | Not applicable |
| Has cross-aggregate transactions (saga, outbox) | Likely | May exist (embedded) |
| Primarily orchestrates I/O or external systems | No | Yes |
| Compile-time layer isolation provides safety | Yes — use separate projects | No — use folder convention |
| Separate team ownership per layer anticipated | Likely | Unlikely |
| Domain model is expected to grow over time | Yes | No |

## Alternatives Considered

- **Uniform 4-project structure for all services:** Rejected. Adds significant build overhead and cognitive friction for I/O pipeline services where no domain invariants exist to protect.
- **Uniform single-project for all services:** Rejected. Removes compile-time architectural enforcement for core domain services where it provides the highest safety value.
- **.NET-only constraint for all MCP servers:** Rejected. The Node.js MCP ecosystem provides mature, community-maintained tooling that would be impractical to replicate in .NET without significant investment and ongoing maintenance burden.
