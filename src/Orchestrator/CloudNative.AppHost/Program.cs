var builder = DistributedApplication.CreateBuilder(args);

// ── Translation API — Minimal API (no DDD, contrast sample) ──────────────────
var translationApi = builder.AddProject<Projects.CloudNative_TranslationApi>("translationapi")
    .WithExternalHttpEndpoints();

// ── Accounting Service — Pure DDD (Clean Architecture) ────────────────────────
var accountingApi = builder.AddProject<Projects.CloudNative_AccountingService_API>("accountingservice")
    .WithExternalHttpEndpoints();

// ── AI Service — Pragmatic DDD ────────────────────────────────────────────────
var aiApi = builder.AddProject<Projects.CloudNative_AIService_API>("aiservice")
    .WithExternalHttpEndpoints();

// ── AI Agent Orchestrator ─────────────────────────────────────────────────────
var agentOrchestrator = builder.AddProject<Projects.CloudNative_Agent_Orchestrator>("agentorchestrator")
    .WithReference(aiApi)
    .WaitFor(aiApi);

// ── Gateway (single entry point) ──────────────────────────────────────────────
var gateway = builder.AddProject<Projects.CloudNative_Gateway>("gateway")
    .WithReference(translationApi)
    .WithReference(accountingApi)
    .WithReference(aiApi)
    .WithExternalHttpEndpoints();

// ── Web Frontend ──────────────────────────────────────────────────────────────
builder.AddProject<Projects.CloudNative_Web>("webfrontend")
    .WithReference(gateway)
    .WaitFor(gateway)
    .WithExternalHttpEndpoints();

builder.Build().Run();
