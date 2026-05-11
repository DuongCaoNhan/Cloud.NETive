# Cloud.NETive

A production-ready **.NET 10 + .NET Aspire 13** solution template demonstrating three levels of **Domain-Driven Design** across cloud-native microservices — from zero layers to strict Clean Architecture.

---

## DDD Spectrum

| Service | Pattern | Description |
|---------|---------|-------------|
| `CloudNative.TranslationApi` | **No DDD** | Single `Program.cs`, no layers — illustrates when DDD is overkill |
| `CloudNative.AIService` | **Pragmatic DDD** | Minimal APIs, direct DbContext, Rich Model lite, vector embedding (SQL Server 2025) |
| `CloudNative.AccountingService` | **Pure DDD** | Private setters, Value Objects (`Money`), Domain Events, MediatR CQRS, FluentValidation pipeline |

> The three services form a deliberate spectrum: **No DDD → Pragmatic DDD → Pure DDD** — choose the right weight for each domain.

---

## Quick Start

```powershell
# 1. Clone or copy the template
git clone https://github.com/your-org/Cloud.NETive.git

# 2. Rename namespace throughout (CloudNative → YourProduct)
.\tools\migration\project-rename\rename-project.ps1 -OldName "CloudNative" -NewName "YourProduct"

# 3. Restore & build
dotnet restore CloudNative.slnx
dotnet build   CloudNative.slnx

# 4. Run via Aspire AppHost
dotnet run --project src/Orchestrator/CloudNative.AppHost
```

Aspire dashboard: `https://localhost:17052`

---

## Solution Structure

```
Cloud.NETive/
├── deploy/
│   ├── configs/                           # Shared appsettings (logging, etc.)
│   └── docker/                            # Docker Compose files
├── ops/
│   ├── monitoring/                        # Prometheus, Grafana configs
│   └── logging/                           # Fluentbit, OTel Collector configs
├── provisioning/
│   ├── cloud/azure/                       # Azure Bicep IaC
│   └── cloud/aws/                         # AWS Terraform IaC
├── scripts/
│   ├── build/                             # Build & validation scripts
│   ├── database/                          # EF Core migration scripts
│   └── deploy/                            # Azure deployment scripts
├── src/
│   ├── Orchestrator/
│   │   └── CloudNative.AppHost/           # .NET Aspire orchestrator & service discovery
│   ├── Agents/
│   │   └── CloudNative.Agent.Orchestrator/ # Semantic Kernel agent orchestration
│   ├── Apps/
│   │   ├── CloudNative.Web/               # Blazor Server web application
│   │   ├── CloudNative.MAUI/              # Cross-platform MAUI Blazor Hybrid
│   │   └── CloudNative.WebCrawler/        # Hybrid web crawling pipeline
│   ├── BuildingBlocks/
│   │   ├── CloudNative.AI.Abstractions/   # IAIService, IEmbeddingService interfaces
│   │   ├── CloudNative.Core/              # Base entities, domain interfaces, shared models
│   │   ├── CloudNative.Data/              # EF Core base DbContext, data persistence helpers
│   │   ├── CloudNative.Security/          # JWT auth, RBAC, AES-256 encryption abstractions
│   │   ├── CloudNative.Messaging/         # Messaging abstractions
│   │   ├── CloudNative.Storage/           # Storage abstractions
│   │   ├── CloudNative.Shared/            # Shared Blazor components
│   │   ├── CloudNative.ServiceDefaults/   # Aspire shared config (OTel, health checks, Serilog)
│   │   ├── CloudNative.Utils/             # Utilities and helpers
│   │   ├── CloudNative.Testing/           # Test helpers and base classes
│   │   ├── EventBus/                      # Event bus infrastructure
│   │   └── IntegrationEventLogEF/         # Transactional outbox (EF Core)
│   ├── Gateways/
│   │   └── CloudNative.Gateway/           # API Gateway (YARP reverse proxy)
│   ├── Services/
│   │   ├── Translation/
│   │   │   └── CloudNative.TranslationApi/            # ★ No DDD — flat Minimal API
│   │   ├── Accounting/
│   │   │   └── CloudNative.AccountingService/
│   │   │       ├── CloudNative.AccountingService.API/           # Controllers → IMediator only
│   │   │       ├── CloudNative.AccountingService.Application/   # Commands, Queries, DTOs, Validators
│   │   │       ├── CloudNative.AccountingService.Domain/        # Entities, Value Objects, Events
│   │   │       └── CloudNative.AccountingService.Infrastructure/ # EF Core, Repository impl
│   │   ├── AI/
│   │   │   └── CloudNative.AIService/
│   │   │       ├── CloudNative.AIService.API/           # Minimal API endpoints
│   │   │       ├── CloudNative.AIService.Application/   # DTOs + service interfaces
│   │   │       ├── CloudNative.AIService.Domain/        # Rich Model lite entities
│   │   │       └── CloudNative.AIService.Infrastructure/ # EF Core, vector embedding
│   │   └── Inventory/
│   │       └── CloudNative.InventoryService/
│   │           └── CloudNative.InventoryService.Application/
│   └── Infrastructure/
│       └── Providers/
│           ├── CloudNative.Infra.Azure/   # Azure cloud provider
│           ├── CloudNative.Infra.AWS/     # AWS cloud provider
│           ├── CloudNative.Infra.GCP/     # Google Cloud provider
│           └── CloudNative.Infra.OnPremise/ # On-premise provider
├── tests/
│   └── Architecture/
│       └── CloudNative.ArchitectureTests/ # NetArchTest layer dependency rules
├── tools/
│   ├── dev/                               # Code metrics, data generators, token counters
│   ├── ops/                               # Package management, security scanning
│   └── migration/                         # Project rename, service migration scripts
└── CloudNative.slnx                       # Solution file (XML format)
```

---

## Aspire Service Graph

Services registered in `CloudNative.AppHost`:

```
WebFrontend (Blazor)
    └── Gateway (YARP)
            ├── TranslationApi
            ├── AccountingService
            └── AIService

Agent.Orchestrator
    └── AIService

(All services expose /health and are wired into the Aspire dashboard)
```

---

## DDD Pattern Comparison

### No DDD — `CloudNative.TranslationApi`

```
Program.cs      Single file: endpoints + records + mock translation provider
                Use when: proxy / utility / thin wrapper — no real business logic
```

### Pragmatic DDD — `CloudNative.AIService`

```
Domain/
  Entities/     public setters for scalars; private for state (IsActive, Embedding)
                factory methods + business methods (UpdateEmbedding, Activate/Deactivate)

Application/
  Model/        AIItemDto, request records, mapping extensions
  Services/     IAIEmbeddingService interface

Infrastructure/
  Persistence/  EF Core DbContext, vector(384) column config + JSON fallback
  Services/     AIEmbeddingService (mock — swap for Azure OpenAI / Ollama)

API/
  Extensions/              AddApplicationServices(), MapAIEndpoints()
  Apis/AIItemsApi.cs       CRUD — inject DbContext + IAIEmbeddingService directly
  Apis/AISearchApi.cs      POST /semantic — cosine similarity search
  Program.cs               3 lines
```

### Pure DDD — `CloudNative.AccountingService`

```
Domain/
  Entities/     Private setters, factory methods, business invariants enforced in entity
  ValueObjects/ Money (immutable, structural equality)
  Events/       IDomainEvent, AccountingItemCreatedDomainEvent
  Exceptions/   AccountingDomainException
  Repositories/ IAccountingRepository (interface only — no EF references)

Application/
  Commands/     CreateAccountingItemCommand + Handler (MediatR)
  Queries/      GetAccountingItemQuery + Handler
  Behaviours/   ValidationBehavior (FluentValidation MediatR pipeline)
  DTOs/         AccountingItemDto — API layer never touches Domain Entities

Infrastructure/
  Persistence/  EF Core DbContext, IEntityTypeConfiguration, Repository implementation

API/
  Controllers/  AccountingController — calls IMediator only, zero business logic
```

---

## Architecture Overview

| Layer | Technology | Notes |
|-------|-----------|-------|
| Orchestration | .NET Aspire 13 | Service discovery, dashboard, health monitoring |
| API Gateway | YARP | Single ingress, routes to all downstream services |
| Pure DDD | Clean Architecture + MediatR | Accounting — enforced layer isolation via NetArchTest |
| Pragmatic DDD | Minimal API + EF Core | AI — direct injection, fewer abstractions |
| No DDD | Single `Program.cs` | Translation — zero overhead for utility services |
| Frontend | Blazor Server + MAUI Hybrid | Shared `CloudNative.Shared` component library |
| AI Agents | Semantic Kernel | Multi-provider: Azure AI Foundry / Ollama / ONNX |
| Security | JWT Bearer + AES-256 | `CloudNative.Security` building block |
| Observability | OpenTelemetry + Prometheus | Traces, metrics, structured logs via `ServiceDefaults` |
| Infrastructure | Multi-cloud providers | Azure / AWS / GCP / On-Premise |

---

## Technology Stack

| Concern | Technology |
|---------|-----------|
| Runtime | .NET 10 |
| Orchestration | .NET Aspire 13 |
| Web UI | Blazor Server |
| Mobile / Desktop | MAUI Blazor Hybrid |
| API Gateway | YARP |
| ORM | Entity Framework Core 10 |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| AI orchestration | Semantic Kernel |
| Logging | Serilog 10 |
| Telemetry | OpenTelemetry 1.14 |
| API Docs | Swashbuckle 10 (OpenApi 2.x) |
| Architecture tests | NetArchTest |
| Unit tests | xUnit |
| IaC | Azure Bicep · AWS Terraform |
| Containers | Docker Compose |

---

## Building Blocks

| Project | Purpose |
|---------|---------|
| `CloudNative.ServiceDefaults` | Registers OpenTelemetry, Serilog, health checks — added to every service |
| `CloudNative.Core` | `BaseEntity`, domain interfaces, shared value types |
| `CloudNative.Data` | EF Core base `DbContext`, soft-delete, audit fields |
| `CloudNative.Security` | `ITokenService` (JWT), `IPermissionService` (RBAC), `IEncryptionService` (AES-256) |
| `CloudNative.AI.Abstractions` | `IAIService`, `IEmbeddingService` |
| `CloudNative.Messaging` | Messaging provider abstraction |
| `CloudNative.Storage` | Blob/file storage abstraction |
| `CloudNative.EventBus` | Event bus (publish / subscribe) infrastructure |
| `IntegrationEventLogEF` | Transactional outbox pattern for EF Core |
| `CloudNative.Shared` | Blazor shared components (layout, nav, etc.) |
| `CloudNative.Utils` | Cross-cutting helpers and extensions |
| `CloudNative.Testing` | Base test classes and test data builders |

---

## Naming Convention

All projects follow: `CloudNative.{Domain?}.{Layer?}`

| Project | Description |
|---------|-------------|
| `CloudNative.AppHost` | Aspire orchestrator |
| `CloudNative.ServiceDefaults` | Shared Aspire configuration |
| `CloudNative.AccountingService.API` | Accounting — presentation layer (Pure DDD) |
| `CloudNative.AccountingService.Domain` | Accounting — domain layer (Pure DDD) |
| `CloudNative.AIService.API` | AI — presentation layer (Pragmatic DDD) |
| `CloudNative.TranslationApi` | Translation — flat Minimal API (No DDD) |
| `CloudNative.Infra.Azure` | Azure infrastructure provider |

To rename `CloudNative` to your own product name:

```powershell
.\tools\migration\project-rename\rename-project.ps1 -OldName "CloudNative" -NewName "Contoso"
```

---

## Environment Configuration

Service control is driven by `AppHost/appsettings.json`:

```json
{
  "Services": {
    "EnableTranslationApi":     true,
    "EnableAccountingService":  true,
    "EnableAIService":          true,
    "EnableInventoryService":   true,
    "EnableAgentOrchestrator":  true,
    "EnableMonitoringService":  true,
    "EnableGateway":            true,
    "EnableWebService":         true
  }
}
```

Or via environment variables:

```powershell
$env:Services__EnableAIService = "false"
dotnet run --project src/Orchestrator/CloudNative.AppHost
```

Or via command-line argument:

```powershell
dotnet run --project src/Orchestrator/CloudNative.AppHost -- --Services:EnableAIService=false
```

---

## Key Scripts

| Script | Purpose |
|--------|---------|
| `scripts/build/build-all.ps1` | Build entire solution |
| `scripts/build/validate-solution-structure.ps1` | Verify all projects are wired in the solution |
| `scripts/database/migrate.ps1` | Run EF Core database migrations |
| `scripts/deploy/deploy-azure.ps1` | Deploy to Azure Container Apps |
| `tools/ops/package-management/security-scan.ps1` | Scan NuGet packages for CVEs |
| `tools/ops/package-management/package-health-check.ps1` | Check for outdated packages |
| `tools/ops/package-management/update-opentelemetry.ps1` | Fix NU1603 OTel version warnings |
