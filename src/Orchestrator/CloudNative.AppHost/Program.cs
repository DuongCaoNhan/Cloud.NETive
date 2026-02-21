using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// ── Configuration-based service control ──────────────────────────────────────
var cfg = builder.Configuration;
bool enable(string key, bool @default = true) =>
    cfg.GetValue<bool>($"Services:Enable{key}", @default);

// ── Core API Service ──────────────────────────────────────────────────────────
var apiService = builder.AddProject<Projects.CloudNative_ApiService>("apiservice")
    .WithExternalHttpEndpoints();

// ── Translation API ───────────────────────────────────────────────────────────
var translationApi = builder.AddProject<Projects.CloudNative_TranslationApi>("translationapi")
    .WithReference(apiService)
    .WaitFor(apiService);

// ── Domain Services ───────────────────────────────────────────────────────────
var accountingApi = builder.AddProject<Projects.CloudNative_AccountingService_API>("accountingservice")
    .WithReference(apiService)
    .WaitFor(apiService);

var aiApi = builder.AddProject<Projects.CloudNative_AIService_API>("aiservice")
    .WithReference(apiService)
    .WaitFor(apiService);

var hrApi = builder.AddProject<Projects.CloudNative_HRService_API>("hrservice")
    .WithReference(apiService)
    .WaitFor(apiService);

var inventoryApi = builder.AddProject<Projects.CloudNative_InventoryService_API>("inventoryservice")
    .WithReference(apiService)
    .WaitFor(apiService);

// ── Monitoring Service ────────────────────────────────────────────────────────
var monitoringService = builder.AddProject<Projects.CloudNative_MonitoringService>("monitoringservice");

// ── AI Agent Orchestrator ─────────────────────────────────────────────────────
var agentOrchestrator = builder.AddProject<Projects.CloudNative_Agent_Orchestrator>("agentorchestrator")
    .WithReference(apiService)
    .WithReference(aiApi)
    .WaitFor(aiApi);

// ── Gateway (single entry point for all APIs) ─────────────────────────────────
var gateway = builder.AddProject<Projects.CloudNative_Gateway>("gateway")
    .WithReference(apiService)
    .WithReference(translationApi)
    .WithReference(accountingApi)
    .WithReference(aiApi)
    .WithReference(hrApi)
    .WithReference(inventoryApi)
    .WithExternalHttpEndpoints();

// ── Web Frontend ──────────────────────────────────────────────────────────────
builder.AddProject<Projects.CloudNative_Web>("webfrontend")
    .WithReference(gateway)
    .WithReference(apiService)
    .WaitFor(gateway)
    .WithExternalHttpEndpoints();

builder.Build().Run();
