import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";

const server = new McpServer({
  name: "cloudnative-tools",
  version: "1.0.0",
});

// TODO: register tools here
// server.tool("tool-name", "description", schema, handler);

const transport = new StdioServerTransport();
await server.connect(transport);
