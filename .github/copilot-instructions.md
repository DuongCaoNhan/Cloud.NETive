<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

###  CURRENT ARCHITECTURE
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

### PACKAGE Management Standards
**ALL PROJECTS MUST FOLLOW PACKAGE MANAGEMENT GUIDELINES**

#### MANDATORY Requirements:
1. **Version Consistency**: Use same versions for related packages
2. **Security First**: Update packages with vulnerabilities immediately  
3. **Aspire Compatibility**: Ensure packages work with .NET Aspire
4. **Documentation**: Follow `.github\instructions\package-management.instructions.md`

#### Automation Tools (use from `tools\ops\package-management\`):
- **update-opentelemetry.ps1**: Fix NU1603 warnings
- **security-scan.ps1**: Detect and fix vulnerabilities
- **package-health-check.ps1**: Overall solution health check

#### Common Package Versions:
```xml
<!-- OpenTelemetry (REQUIRED 1.14.0+) -->
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.14.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.14.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.14.0" />

<!-- Serilog -->
<PackageReference Include="Serilog.AspNetCore" Version="10.0.0" />

<!-- Swagger/OpenAPI (uses Microsoft.OpenApi 2.x - namespace: Microsoft.OpenApi, NOT Microsoft.OpenApi.Models) -->
<PackageReference Include="Swashbuckle.AspNetCore" Version="10.0.1" />

<!-- Aspire Entity Framework Core -->
<PackageReference Include="Aspire.Microsoft.EntityFrameworkCore.SqlServer" Version="13.1.1" />
```

#### FORBIDDEN:
- ❌ Mixed versions of same package family
- ❌ Packages with known vulnerabilities  
- ❌ Preview packages in production projects
- ❌ `Microsoft.AspNetCore.OpenApi` — causes `TypeLoadException` when used alongside Swashbuckle (OpenApi v1.x vs v2.x conflict)
- ❌ `using Microsoft.OpenApi.Models` — use `using Microsoft.OpenApi` (namespace changed in OpenApi 2.x)

### DEVELOPMENT GUIDELINES
- **Framework**: .NET 10
- **Aspire Version**: 13 (latest stable with CLI GA)
- **Architecture**: Clean Architecture (API/Application/Domain/Infrastructure) per domain service
- **Solution Format**: .slnx (XML format, 39 projects)
- **UI Framework**: Blazor Server + MAUI Blazor Hybrid
- **API Gateway**: YARP reverse proxy
- **Styling**: Bootstrap 5 with responsive design
- **Cloud Providers**: Azure, AWS, GCP, On-Premise (multi-cloud via Infrastructure/Providers)
- **Tools**: PowerShell automation, Python validation scripts
- **AI Stack**: Microsoft Agent Framework + LangChain + Azure AI Foundry + Ollama + ONNX
- **Orchestration**: .NET Aspire with custom service control features
- **IDE**: Visual Studio 2026 or VS Code with C# Dev Kit

### SPECIALIZED INSTRUCTION REFERENCES
For domain-specific development, follow these detailed guidelines:

#### C# Coding Conventions
- **File**: `.github/instructions/csharp-coding-conventions.instructions.md`
- **Scope**: Code style, naming conventions, best practices
- **Standards**: Modern C# features, documentation requirements

### DOCUMENTATION STYLE GUIDELINES

#### Content Formatting
- **Clear Structure**: Use proper heading levels (H1, H2, H3) for hierarchy
- **Code Examples**: Always include practical examples in code blocks with proper syntax highlighting
- **Consistency**: Maintain consistent terminology throughout documentation
- **Conciseness**: Prefer direct language and avoid redundancy

#### Visual Elements
- **Limited Emoji Usage**: Avoid excessive emojis; use only when essential for emphasis
- **No Memojis**: Do not use memojis in technical documentation
- **Appropriate Formatting**: Use bold, italics, and code formatting instead of emojis for emphasis
- **Tables Over Icons**: Prefer structured tables for comparisons rather than icon-based lists
- **When to Use Emojis**: Only for status indicators (✅❌⚠️) or critical callouts

#### Acceptable Use Cases
- Status indicators in checklists: ✅ Complete, ❌ Incomplete, ⚠️ Warning
- Critical warnings or alerts: ⚠️ Security Warning
- Simple section dividers where appropriate: For lists or technical sections

#### Preferred Alternatives to Emojis
- Use **bold text** for emphasis instead of emoji indicators
- Use `code formatting` for technical terms instead of computer emojis
- Use > blockquotes for important notes instead of information emojis
- Use bullet points for lists instead of emoji bullets
- Use proper headings for section titles instead of emoji prefixes

#### Documentation Examples
```

These guidelines ensure documentation remains professional, consistent, and accessible while reducing unnecessary visual clutter.

### CUSTOM ASPIRE FEATURES
- **Dynamic Service Control**: Enable/disable services via configuration
- **Smart Dependency Management**: Conditional service references
- **Environment-Based Deployment**: Different service combinations per environment
- **Enhanced Monitoring**: Custom logging and service status tracking
- **Flexible Orchestration**: Runtime service management capabilities

#### Service Control Configuration Examples:
```json
// appsettings.json
{
  "Services": {
    "EnableApiService": false,  //  Disable API Service
    "EnableWebService": true    //  Only run Web Service
  }
}
```

```powershell
# Environment Variables
$env:Services__EnableApiService = "false"
$env:Services__EnableWebService = "true"
dotnet run --project DataNative.AppHost

# Command Line Arguments
dotnet run --project DataNative.AppHost -- --Services:EnableApiService=false
```

#### Current Aspire Status:
- **Version**: 13 ✅
- **Dashboard**: https://dev.datanative.ai:17052 ✅  
- **CLI**: GA (General Availability) ✅
- **Templates**: Updated to 13 ✅
- **SDK Integration**: Aspire.AppHost.Sdk configured ✅

---

##  API DEVELOPMENT REQUIREMENTS & STANDARDS

###  MANDATORY API Architecture Standards
- **Clean Architecture**: Controller → Service → Repository pattern
- **Dependency Injection**: Register all services in Program.cs
- **ServiceDefaults**: Always reference DataNative.ServiceDefaults
- **Swagger/OpenAPI**: Use Swashbuckle.AspNetCore v10.0.1+ for .NET 10 compatibility (NEVER use Microsoft.AspNetCore.OpenApi)
- **Health Checks**: Implement `/health` endpoint with detailed checks
- **Versioning**: Use URL versioning (e.g., `/api/v1/`) for all endpoints

###  REQUIRED Documentation
- **XML Documentation**: `/// <summary>` for all public methods and controllers
- **OpenAPI Attributes**: `[HttpGet]`, `[ProducesResponseType]`, `[SwaggerOperation]`
- **README.md**: Setup instructions, endpoints documentation, examples
- **API Models**: Data contracts with validation attributes

###  SECURITY Requirements
- **Authentication**: JWT Bearer tokens or API Keys
- **Authorization**: Role-based access control with `[Authorize]` attributes
- **Input Validation**: Use Data Annotations and ModelState validation
- **CORS**: Configure specific origins, avoid AllowAnyOrigin in production
- **HTTPS**: Enforce HTTPS redirection (smart logic for development)

### PERFORMANCE & Monitoring
- **Caching**: Implement response caching where appropriate
- **Logging**: Structured logging with ILogger<T> and semantic log events
- **Telemetry**: OpenTelemetry integration via ServiceDefaults
- **Rate Limiting**: Implement for public endpoints
- **Exception Handling**: Global exception middleware with proper error responses

### ASPIRE Integration Standards
- **Project Structure**: Follow DataNative.{ServiceName} naming
- **ServiceDefaults**: Always call `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`
- **Configuration**: Use `IOptions<T>` pattern for settings
- **Service Discovery**: Register services in AppHost for cross-service communication
- **Environment Configs**: Support Development, Staging, Production environments

### PACKAGE Standards
- **Swashbuckle.AspNetCore**: v10.0.1+ (compatible with .NET 10, uses Microsoft.OpenApi 2.x)
- **FORBIDDEN**: `Microsoft.AspNetCore.OpenApi` — causes TypeLoadException with Swashbuckle (OpenApi v1.x vs v2.x conflict)
- **Namespace**: Use `using Microsoft.OpenApi;` (NOT `using Microsoft.OpenApi.Models;` which is v1.x)
- **Logging**: Serilog.AspNetCore v10.0.0 for advanced logging scenarios
- **Aspire EF Core**: Aspire.Microsoft.EntityFrameworkCore.SqlServer v13.1.1
- **Validation**: FluentValidation or Data Annotations
- **HTTP Clients**: Use typed HttpClient with service discovery

### CODE Quality Standards
- **Async/Await**: Use async patterns for I/O operations
- **Response Types**: Return `ActionResult<T>` or `IActionResult`
- **Error Handling**: Consistent error response format with problem details
- **Testing**: Unit tests for business logic, integration tests for endpoints
- **Naming**: RESTful conventions (GET /api/v1/users, POST /api/v1/users)

### DEPLOYMENT Standards
- **launchSettings.json**: HTTP-only for Aspire compatibility
- **appsettings**: Environment-specific configurations
- **Docker**: Containerization support via Aspire
- **CI/CD**: Build validation and automated testing

### ASPIRE INTEGRATION REQUIREMENTS
**ALL NEW SERVICES/APIs MUST BE INTEGRATED INTO ASPIRE ORCHESTRATION**

#### MANDATORY Steps for New Services:
1. **Project Setup**: Follow `DataNative.{ServiceName}` naming convention
2. **ServiceDefaults Reference**: Add ProjectReference to `DataNative.ServiceDefaults`
3. **Common Configuration**: Follow `common-configuration-guidelines.instructions.md` for centralized config
4. **Program.cs Integration**: Add `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`
5. **AppHost Registration**: Add service to `DataNative.AppHost/Program.cs`
6. **Service Control**: Implement conditional enablement via configuration
7. **Follow Instructions**: Use appropriate specialized instruction file for service type

#### AppHost Registration Pattern:
```csharp
// Add to CloudNative.AppHost/Program.cs
var newService = builder.AddProject<Projects.CloudNative_NewService>("newservice")
    .WithReference(apiService)  // If needs API access
    .WithReference(webCrawler); // If needs WebCrawler access

// For Web services - add references
webService.WithReference(apiService)
          .WithReference(newService);
```

#### Service Control Configuration:
```csharp
// Add to configuration sections
var enableNewService = builder.Configuration.GetValue<bool>("Services:EnableNewService", true);
if (enableNewService)
{
    var newService = builder.AddProject<Projects.CloudNative_NewService>("newservice");
    Console.WriteLine(" Registered: New Service");
}
```

#### Required Configuration Files:
- **launchSettings.json**: HTTP-only profiles for Aspire compatibility
- **appsettings.json**: Environment-specific service settings
- **Health Checks**: `/health` endpoint for Aspire monitoring
- **Service Discovery**: Enable communication between services

#### FORBIDDEN Practices:
- **NO standalone services** - All must integrate with Aspire
- **NO hardcoded URLs** - Use service discovery
- **NO mixed HTTP/HTTPS** in development with Aspire
- **NO manual port assignment** - Let Aspire manage ports
