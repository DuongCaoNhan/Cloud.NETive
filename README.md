# CloudNative Solution Template

A ready-to-use **.NET 10 + .NET Aspire 13** solution template following **Clean Architecture**, **Domain-Driven Design**, and **cloud-native microservices** patterns.

---

## Updated Solution Structure

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
