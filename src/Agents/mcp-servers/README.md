# MCP Servers

Node.js MCP (Model Context Protocol) servers for Cloud.NETive tooling.

These servers are registered in .NET Aspire as external executable resources and communicate over stdio or SSE transport. See [ADR-001](../../docs/ADRs/001-service-architecture-patterns.md) for the rationale behind the polyglot runtime decision.

## Servers

| Server | Description |
|--------|-------------|
| `cloudnative-tools/` | General Cloud.NETive development tools |

## Running via Aspire

MCP servers are started automatically by the AppHost. To run a server standalone for local development:

```bash
cd cloudnative-tools
npm install
npm start
```

## Adding a New Server

1. Create a new subfolder under `mcp-servers/`.
2. Initialise a Node.js package (`npm init`).
3. Add the `@modelcontextprotocol/sdk` dependency.
4. Register the server in `src/Orchestrator/CloudNative.AppHost/Program.cs` using `builder.AddNpmApp(...)`.
