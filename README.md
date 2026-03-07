# CloudNative Solution Template

A ready-to-use **.NET 10 + .NET Aspire 13** solution template demonstrating three levels of **Domain-Driven Design** applied to cloud-native microservices.

---

## DDD Sample Services

| Service | Pattern | Description |
|---------|---------|-------------|
| `CloudNative.AccountingService` | **Pure DDD** | Strict Clean Architecture: private setters, Value Objects (`Money`), Domain Events, MediatR CQRS, FluentValidation pipeline |
| `CloudNative.AIService` | **Pragmatic DDD** | Flat structure: Minimal APIs, direct DbContext injection, Rich Model lite, vector embedding with SQL Server 2025 support |
| `CloudNative.TranslationApi` | **No DDD (Minimal API)** | Single `Program.cs`, no layers — illustrates when DDD is overkill |

> These three services form a spectrum: **No DDD → Pragmatic DDD → Pure DDD** — showing when to apply each approach.

---

## Solution Structure

```
CloudNative/
├── deploy/                                # Runtime artifacts (Docker, configs)
│   ├── configs/                           # Shared app settings
│   └── docker/                            # Docker Compose files
├── docs/                                  # Documentation & guides
├── ops/                                   # Operations (monitoring, logging)
│   ├── monitoring/                        # Prometheus, Grafana configs
│   └── logging/                           # Fluentbit, OTel configs
├── provisioning/                          # IaC (Azure Bicep, AWS Terraform)
├── scripts/                               # Database & deploy scripts
├── src/
│   ├── Orchestrator/
│   │   └── CloudNative.AppHost/              # Aspire orchestrator & service discovery
│   ├── Agents/
│   │   └── CloudNative.Agent.Orchestrator/   # Semantic Kernel agent orchestration
│   ├── Apps/
│   │   ├── CloudNative.Web/                  # Blazor Server web application
│   │   └── CloudNative.MAUI/                 # Cross-platform MAUI Blazor Hybrid
│   ├── BuildingBlocks/
│   │   ├── CloudNative.AI.Abstractions/      # AI/ML abstractions & interfaces
│   │   ├── CloudNative.Core/                 # Domain models, core entities
│   │   ├── CloudNative.ServiceDefaults/      # Aspire shared configs
│   │   ├── CloudNative.Shared/               # Shared Blazor components
│   │   ├── CloudNative.Messaging/            # Messaging abstractions
│   │   ├── CloudNative.Storage/              # Storage abstractions
│   │   ├── CloudNative.Security/             # Security abstractions
│   │   ├── CloudNative.Utils/                # Utilities and helpers
│   │   ├── CloudNative.EventBus/             # Event bus infrastructure
│   │   └── CloudNative.IntegrationEventLogEF/ # Integration event logging
│   ├── Gateways/
│   │   └── CloudNative.Gateway/              # API Gateway (YARP reverse proxy)
│   ├── Services/
│   │   ├── Translation/                      # ★ No DDD — Minimal API flat structure
│   │   │   └── CloudNative.TranslationApi/
│   │   ├── Accounting/                       # ★ Pure DDD — CQRS + MediatR + Value Objects
│   │   │   └── CloudNative.AccountingService/
│   │   │       ├── .API/                     #   Controllers → MediatR only
│   │   │       ├── .Application/             #   Commands / Queries / DTOs / Validators
│   │   │       ├── .Domain/                  #   Entities, Value Objects, Domain Events
│   │   │       └── .Infrastructure/          #   EF Core, Repository impl
│   │   └── AI/                               # ★ Pragmatic DDD — Minimal APIs + direct DbContext
│   │       └── CloudNative.AIService/
│   │           ├── .API/                     #   Minimal API endpoints (Apis/, Extensions/)
│   │           ├── .Application/             #   Model/ DTOs + Service interfaces (mixed)
│   │           ├── .Domain/                  #   Rich Model lite (some public setters)
│   │           └── .Infrastructure/          #   EF Core, vector embedding service
│   └── Infrastructure/
│       └── Providers/
│           ├── CloudNative.Infra.Azure/      # Azure cloud provider
│           ├── CloudNative.Infra.AWS/        # AWS cloud provider
│           ├── CloudNative.Infra.GCP/        # Google Cloud provider
│           └── CloudNative.Infra.OnPremise/  # On-premise provider
├── tests/
│   └── Architecture/
│       └── CloudNative.ArchitectureTests/    # NetArchTest dependency rules
├── tools/
│   ├── dev/                               # Developer tools (metrics, generators)
│   ├── ops/                               # Operations tools (packages, security)
│   └── migration/                         # One-off migration scripts
└── CloudNative.slnx                       # Solution file (XML format)
```

---

## Quick Start

```powershell
# 1. Copy template to a new location
cp -r CloudNative/ ../MyNewSolution/

# 2. Rename namespace throughout (CloudNative → YourProduct)
.\tools\migration\project-rename\rename-project.ps1 -OldName "CloudNative" -NewName "YourProduct"

# 3. Restore & build
dotnet restore CloudNative.slnx
dotnet build   CloudNative.slnx

# 4. Run via Aspire AppHost
dotnet run --project src/Orchestrator/CloudNative.AppHost
```

---

## Architecture Patterns

| Layer | Pattern | Notes |
|-------|---------|-------|
| Orchestration | .NET Aspire 9.5 AppHost | Service discovery, dashboard, health monitoring |
| API Gateway | YARP reverse proxy | Single ingress, routes to all downstream services |
| **Accounting** | Pure DDD + Clean Architecture | API → MediatR → Application → Domain → Infrastructure |
| **AI** | Pragmatic DDD + Minimal API | Minimal APIs inject DbContext/Services directly, vector search |
| **Translation** | Flat Minimal API | No layers — single Program.cs, proxy/utility pattern |
| Frontend | Blazor Server + MAUI Hybrid | Shared component library |
| AI Agents | Semantic Kernel | Microsoft Agent Framework, multi-provider |
| Observability | OpenTelemetry + Prometheus | Traces, metrics, structured logs |
| Infrastructure | Multi-cloud providers | Azure / AWS / GCP / On-Premise |

---

## DDD Pattern Comparison

### Pure DDD — `CloudNative.AccountingService`

```
Domain/
  Entities/     private setters, factory method, business methods
  ValueObjects/ Money (immutable, equality by value)
  Events/       IDomainEvent, AccountingItemCreatedDomainEvent
  Exceptions/   AccountingDomainException
  Repositories/ IAccountingRepository (interface only)

Application/
  Commands/     CreateAccountingItemCommand + Handler + Validator
  Queries/      GetAccountingItemQuery + Handler
  Behaviours/   ValidationBehavior (MediatR pipeline)
  DTOs/         AccountingItemDto — API never sees Entities

Infrastructure/
  Persistence/  EF Core DbContext, IEntityTypeConfiguration, Repository impl

API/
  Controllers/  AccountingController — calls IMediator only, no DbContext
```

### Pragmatic DDD — `CloudNative.AIService`

```
Domain/
  Entities/     public setters for scalars, private for state (IsActive, Embedding)
                factory + business methods (UpdateEmbedding, Activate/Deactivate)

Application/    "Model/" — DTOs + Interfaces co-exist (no strict separation)
  Model/        AIItemDto, request records, mapping extensions — all in one file
  Services/     IAIEmbeddingService interface

Infrastructure/
  Persistence/  EF Core DbContext, vector(384) config + JSON fallback
  Services/     AIEmbeddingService (mock → swap for Azure OpenAI / Ollama)

API/
  GlobalUsings.cs          common namespaces in one file
  Extensions/              AddApplicationServices(), MapAIEndpoints()
  Apis/AIItemsApi.cs       CRUD — inject AIDbContext + IAIEmbeddingService directly
  Apis/AISearchApi.cs      POST /semantic — cosine similarity search
  Program.cs               3 lines
```

### No DDD — `CloudNative.TranslationApi`

```
Program.cs      Single file: endpoint + records + mock provider
                Use when: proxy/utility, no business logic, thin wrapper
```

---

## Technology Stack

| Concern | Technology |
|---------|-----------|
| Runtime | .NET 10 |
| Orchestration | .NET Aspire 9.5 |
| Web UI | Blazor Server |
| Mobile / Desktop | MAUI Blazor Hybrid |
| API Gateway | YARP |
| ORM | Entity Framework Core 10 |
| CQRS | MediatR 12 (Accounting) |
| Validation | FluentValidation 11 (Accounting) |
| AI orchestration | Semantic Kernel |
| Logging | Serilog 10 |
| Telemetry | OpenTelemetry 1.14 |
| API Docs | Swashbuckle 10 (OpenApi 2.x) |
| Testing | xUnit + NetArchTest |
| IaC | Azure Bicep + AWS Terraform |
| Containers | Docker Compose |

---

## Naming Convention

All projects follow the pattern: `{ProductName}.{Layer}.{Domain?}.{Sublayer?}`

| Example | Description |
|---------|-------------|
| `CloudNative.AppHost` | Aspire orchestrator |
| `CloudNative.ServiceDefaults` | Shared Aspire configuration |
| `CloudNative.AccountingService.API` | Accounting — Presentation layer (Pure DDD) |
| `CloudNative.AccountingService.Domain` | Accounting — Domain layer (Pure DDD) |
| `CloudNative.AIService.API` | AI — Presentation layer (Pragmatic DDD) |
| `CloudNative.TranslationApi` | Translation — Flat Minimal API (No DDD) |
| `CloudNative.Infra.Azure` | Azure provider implementation |

To rename `CloudNative` to your product name:

```powershell
.\tools\migration\project-rename\rename-project.ps1 -OldName "CloudNative" -NewName "Contoso"
```

---

## Key Scripts

| Script | Purpose |
|--------|---------|
| `scripts/build/build-all.ps1` | Build entire solution |
| `scripts/build/validate-solution-structure.ps1` | Verify all projects are wired up |
| `scripts/database/migrate.ps1` | Run EF Core database migrations |
| `scripts/deploy/deploy-azure.ps1` | Deploy to Azure Container Apps |
| `tools/ops/package-management/security-scan.ps1` | Scan NuGet packages for CVEs |
| `tools/ops/package-management/package-health-check.ps1` | Check for outdated packages |

---

## Environment Configuration

Service control is driven by `appsettings.json` in the AppHost:

```json
{
  "Services": {
    "EnableTranslationApi":    true,
    "EnableAccountingService": true,
    "EnableAIService":         true
  }
}
```

Or via environment variables:

```powershell
$env:Services__EnableAIService = "false"
dotnet run --project src/Orchestrator/CloudNative.AppHost
```


```
DataNative/
├── deploy/                                # Runtime artifacts (Docker, configs)
│   ├── configs/                           # Shared app settings
│   └── docker/                            # Docker Compose files
├── docs/                                  # Documentation & guides
├── ops/                                   # Operations (monitoring, logging)
│   ├── monitoring/                        # Prometheus, Grafana configs
│   └── logging/                           # Fluentbit, OTel configs
├── provisioning/                          # IaC (Azure Bicep, AWS Terraform)
├── scripts/                               # Database & deploy scripts
├── src/
│   ├── Orchestrator/
│   │   └── CloudNative.AppHost/              # Aspire orchestrator & service discovery
│   ├── Agents/
│   │   └── CloudNative.Agent.Orchestrator/   # Semantic Kernel agent orchestration
│   ├── Apps/
│   │   ├── CloudNative.Web/                  # Blazor Server web application
│   │   ├── CloudNative.MAUI/                 # Cross-platform MAUI Blazor Hybrid
│   │   ├── CloudNative.WebCrawler/           # Hybrid web crawling pipeline
│   │   └── CloudNative.BilingualTranslator/  # Chrome Extension (Manifest V3)
│   ├── BuildingBlocks/
│   │   ├── CloudNative.AI.Abstractions/      # AI/ML abstractions & interfaces
│   │   ├── CloudNative.Core/                 # Domain models, core entities
│   │   ├── CloudNative.Data/                 # EF Core, data persistence
│   │   ├── CloudNative.Shared/               # Shared Blazor components
│   │   ├── CloudNative.ServiceDefaults/      # Aspire shared configs
│   │   ├── CloudNative.Messaging/            # Messaging abstractions
│   │   ├── CloudNative.Storage/              # Storage abstractions
│   │   ├── CloudNative.Utils/                # Utilities and helpers
│   │   ├── CloudNative.EventBus/            # Event bus infrastructure
│   │   └── CloudNative.IntegrationEventLogEF/ # Integration event logging
│   ├── Gateways/
│   │   └── CloudNative.Gateway/              # API Gateway (YARP reverse proxy)
│   ├── Services/
│   │   ├── WebApi/                        # Aggregator/BFF
│   │   │   └── CloudNative.ApiService/       # Core REST API service
│   │   ├── Translation/                   # Translation domain
│   │   │   └── CloudNative.TranslationApi/   # Translation API service
│   │   ├── Accounting/                    # Clean Architecture (API/Application/Domain/Infrastructure)
│   │   ├── AI/                            # Clean Architecture (API/Application/Domain/Infrastructure)
│   │   ├── HR/                            # Clean Architecture (API/Application/Domain/Infrastructure)
│   │   ├── Inventory/                     # Clean Architecture (API/Application/Domain/Infrastructure)
│   │   ├── Ingestion/                     # Data ingestion pipeline
│   │   ├── Monitoring/
│   │   │   └── CloudNative.MonitoringService/ # Observability & metrics
│   │   └── Audit/                         # Audit trail service
│   └── Infrastructure/
│       └── Providers/
│           ├── CloudNative.Infra.Azure/      # Azure cloud provider
│           ├── CloudNative.Infra.AWS/        # AWS cloud provider
│           ├── CloudNative.Infra.GCP/        # Google Cloud provider
│           └── CloudNative.Infra.OnPremise/  # On-premise provider
├── tests/                                 # Unit, integration, architecture tests
│   ├── Architecture/                      # NetArchTest dependency rules
│   ├── Unit/                              # xUnit unit tests
│   └── Integration/                       # Integration tests
├── tools/                                 # Dev utilities & automation
│   ├── dev/                               # Developer tools (metrics, generators)
│   ├── ops/                               # Operations tools (packages, security)
│   └── migration/                         # One-off migration scripts
└── CloudNative.slnx                          # Solution file (XML format)
```

---

## Quick Start

```powershell
# 1. Copy template to a new location
cp -r Template/ ../MyNewSolution/

# 2. Rename namespace throughout (CloudNative → YourProduct)
.\tools\migration\project-rename\rename-project.ps1 -OldName "CloudNative" -NewName "YourProduct"

# 3. Restore & build
dotnet restore CloudNative.slnx
dotnet build   CloudNative.slnx

# 4. Run via Aspire AppHost
dotnet run --project src/Orchestrator/CloudNative.AppHost
```

---

## Architecture Patterns

| Layer | Pattern | Notes |
|-------|---------|-------|
| Orchestration | .NET Aspire 13.1.1 AppHost | Service discovery, dashboard, health monitoring |
| API Gateway | YARP reverse proxy | Single ingress, route to downstream services |
| Domain Services | Clean Architecture | API → Application → Domain → Infrastructure |
| Frontend | Blazor Server + MAUI Hybrid | Shared component library |
| AI | Semantic Kernel agents | Microsoft Agent Framework, multi-provider |
| Messaging | Event bus + outbox | Transactional message publishing |
| Observability | OpenTelemetry + Prometheus | Traces, metrics, structured logs |
| Infrastructure | Multi-cloud providers | Azure / AWS / GCP / On-Premise |

---

## Technology Stack

| Concern | Technology |
|---------|-----------|
| Runtime | .NET 10 |
| Orchestration | .NET Aspire 13.1.1 |
| Web UI | Blazor Server |
| Mobile / Desktop | MAUI Blazor Hybrid |
| API Gateway | YARP |
| ORM | Entity Framework Core 10 |
| AI orchestration | Semantic Kernel |
| Logging | Serilog 10 |
| Telemetry | OpenTelemetry 1.14 |
| API Docs | Swashbuckle 10 (OpenApi 2.x) |
| Testing | xUnit + NetArchTest |
| IaC | Azure Bicep + AWS Terraform |
| Containers | Docker Compose |

---

## Naming Convention

All projects follow the pattern: `{ProductName}.{Layer}.{Domain?}.{Sublayer?}`

| Example | Description |
|---------|-------------|
| `CloudNative.AppHost` | Aspire orchestrator |
| `CloudNative.ServiceDefaults` | Shared Aspire configuration |
| `CloudNative.AccountingService.API` | Accounting — API layer |
| `CloudNative.AccountingService.Domain` | Accounting — Domain layer |
| `CloudNative.Infra.Azure` | Azure provider implementation |

To rename `CloudNative` to your product name:

```powershell
.\tools\migration\project-rename\rename-project.ps1 -OldName "CloudNative" -NewName "Contoso"
```

---

## Key Scripts

| Script | Purpose |
|--------|---------|
| `scripts/build/build-all.ps1` | Build entire solution |
| `scripts/build/validate-solution-structure.ps1` | Verify all projects are wired up |
| `scripts/database/migrate.ps1` | Run EF Core database migrations |
| `scripts/deploy/deploy-azure.ps1` | Deploy to Azure Container Apps |
| `tools/ops/package-management/security-scan.ps1` | Scan NuGet packages for CVEs |
| `tools/ops/package-management/package-health-check.ps1` | Check for outdated packages |

---

## Environment Configuration

Service control is driven by `appsettings.json` in the AppHost:

```json
{
  "Services": {
    "EnableApiService": true,
    "EnableWebService": true,
    "EnableGateway": true
  }
}
```

Or via environment variables:

```powershell
$env:Services__EnableApiService = "false"
dotnet run --project src/Orchestrator/CloudNative.AppHost
```
