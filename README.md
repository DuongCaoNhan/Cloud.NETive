# CloudNative Solution Template

A ready-to-use **.NET 10 + .NET Aspire 13** solution template following **Clean Architecture**, **Domain-Driven Design**, and **cloud-native microservices** patterns.

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

## Solution Structure

```
Template/
├── CloudNative.slnx                   # Lightweight XML solution file (.NET SDK 17.x+)
│
├── src/
│   ├── Orchestrator/                  # Aspire AppHost — single entry point for all services
│   │   └── CloudNative.AppHost/
│   │
│   ├── BuildingBlocks/                # Shared libraries consumed by all services
│   │   ├── CloudNative.ServiceDefaults/   # Aspire telemetry, health checks, service discovery
│   │   ├── CloudNative.Core/              # Domain models, base entities, value objects
│   │   ├── CloudNative.Shared/            # Shared Blazor UI components (Razor SDK)
│   │   ├── CloudNative.Data/              # EF Core DbContext, repositories, migrations
│   │   ├── CloudNative.Utils/             # Extension methods, helpers
│   │   ├── CloudNative.Security/          # Auth abstractions, JWT helpers
│   │   ├── CloudNative.Testing/           # Shared test utilities and base classes
│   │   ├── CloudNative.AI.Abstractions/   # AI/ML provider interfaces
│   │   ├── CloudNative.Messaging/         # Messaging contracts and abstractions
│   │   ├── CloudNative.Storage/           # Blob/file storage abstractions
│   │   ├── EventBus/                      # In-process and distributed event bus
│   │   └── IntegrationEventLogEF/         # Outbox pattern with EF Core
│   │
│   ├── Apps/                          # Frontend and client applications
│   │   ├── CloudNative.Web/               # Blazor Server
│   │   ├── CloudNative.MAUI/              # MAUI Blazor Hybrid (Win/Android/iOS/macOS)
│   │   └── CloudNative.WebCrawler/        # Web crawling pipeline API
│   │
│   ├── Gateways/                      # API Gateway / reverse proxy
│   │   └── CloudNative.Gateway/           # YARP reverse proxy — single ingress point
│   │
│   ├── Services/                      # Microservices (one folder per domain)
│   │   ├── WebApi/                        # REST aggregator / BFF
│   │   ├── Translation/                   # Translation domain service
│   │   ├── Data/                          # Data access service
│   │   ├── Ingestion/                     # Data ingestion pipeline
│   │   ├── Monitoring/                    # Observability and metrics service
│   │   ├── Accounting/                    # Clean Architecture (API/Application/Domain/Infrastructure)
│   │   ├── AI/                            # Clean Architecture (API/Application/Domain/Infrastructure)
│   │   ├── HR/                            # Clean Architecture (API/Application/Domain/Infrastructure)
│   │   └── Inventory/                     # Clean Architecture (API/Application/Domain/Infrastructure)
│   │
│   ├── Agents/                        # AI agent orchestration
│   │   └── CloudNative.Agent.Orchestrator/  # Semantic Kernel agent workflows
│   │
│   └── Infrastructure/                # Cloud provider implementations
│       └── Providers/
│           ├── CloudNative.Infra.Azure/
│           ├── CloudNative.Infra.AWS/
│           ├── CloudNative.Infra.GCP/
│           └── CloudNative.Infra.OnPremise/
│
├── tests/
│   └── Architecture/
│       └── CloudNative.ArchitectureTests/  # NetArchTest dependency rule validation
│
├── deploy/                            # Runtime artifacts
│   ├── configs/                       # Shared appsettings, logging config
│   └── docker/                        # Docker Compose files (compose/ and legacy/)
│
├── provisioning/                      # Infrastructure as Code
│   ├── cloud/
│   │   ├── azure/uat/                 # Azure Bicep templates
│   │   └── aws/uat/                   # AWS Terraform
│   └── onpremise/                     # On-premise Docker Compose
│
├── ops/                               # Operations configuration
│   ├── monitoring/                    # Prometheus scrape config
│   └── logging/                       # Fluent Bit config
│
├── scripts/                           # Automation scripts
│   ├── build/                         # Build, validate, update solution structure
│   ├── database/                      # EF Core migration runner
│   └── deploy/                        # Azure deployment automation
│
└── tools/                             # Developer and ops tooling
    ├── dev/
    │   ├── code-metrics/              # Lines-of-code counter
    │   ├── data-generator/            # Test data generation (CSV/Excel)
    │   └── token-counter/             # LLM token cost estimator
    ├── migration/
    │   ├── project-rename/            # Namespace rename across solution
    │   ├── service-migration/         # Migrate service to Clean Architecture
    │   ├── upgrade/                   # .NET / Aspire version upgrade scripts
    │   └── url-migration/             # Switch endpoint URLs between environments
    └── ops/
        ├── package-management/        # NuGet health check, security scan, OTel updater
        └── security/                  # Sensitive info detector + git hooks
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
