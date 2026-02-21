using CloudNative.AI.Abstractions;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "CloudNative Agent Orchestrator", Version = "v1" }));

// Semantic Kernel — configure your preferred AI provider here
// builder.Services.AddKernel()
//     .AddOpenAIChatCompletion("gpt-4o", builder.Configuration["AI:OpenAI:ApiKey"]!);
// Or for Ollama (local):
// builder.Services.AddKernel()
//     .AddOllamaChatCompletion("llama3.2", new Uri("http://localhost:11434"));

var app = builder.Build();
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1/agent");

api.MapPost("/chat", (ChatRequest req) =>
{
    // TODO: inject IKernel and invoke a Semantic Kernel agent pipeline
    return new ChatResponse($"Echo: {req.Message}", "agent-orchestrator", DateTime.UtcNow);
})
.WithName("Chat")
.WithSummary("Send a message to the AI agent");

api.MapGet("/plugins", () => new[]
{
    new { name = "DataPlugin",        description = "Query and analyse business data" },
    new { name = "TranslationPlugin", description = "Translate text via Translation API" },
    new { name = "ReportPlugin",      description = "Generate executive reports" }
})
.WithName("ListPlugins")
.WithSummary("List available Semantic Kernel plugins");

app.Run();

record ChatRequest(string Message, string? SystemPrompt = null, string? SessionId = null);
record ChatResponse(string Reply, string Agent, DateTime Timestamp);
