# ProductOrderAPI

## Overview
- ProductOrderAPI is a .NET 8 Web API for managing products and orders.
- Implements Clean Architecture with CQRS using MediatR, EF Core (PostgreSQL), and Swagger.
- Focus areas: clear separation of concerns, testability, maintainability, and consistent error handling.

## Tech Stack
- .NET 8, ASP.NET Core Web API
- MediatR (CQRS commands/queries)
- FluentValidation (request validation)
- AutoMapper (DTO mapping)
- Entity Framework Core + Npgsql (PostgreSQL provider)
- Swashbuckle (OpenAPI/Swagger)
- xUnit + Moq (unit tests)

## Architecture
- Onion Architecture (Clean Architecture-inspired):
  - Inner ring — `Domain`: core entities, value objects, enums, domain events, domain exceptions. Pure C# with no external dependencies.
  - Middle ring — `Application`: use cases (commands/queries), DTOs, validators, pipeline behaviors. References `Domain` only and defines interfaces for infrastructure abstractions.
  - Outer ring — `Infrastructure`: EF Core DbContext, configurations, repositories, services, migrations. Implements `Application` interfaces and depends on external packages (EF, Npgsql).
  - Outer ring — `API`: ASP.NET Core host, controllers, middleware, DI wiring, Swagger. Depends on `Application` and `Infrastructure` and orchestrates the system.
- CQRS with MediatR:
  - Commands mutate state (e.g., create/update/delete product)
  - Queries read state (e.g., get product by id)
- Validation pipeline:
  - All MediatR requests pass through `ValidationBehavior<TRequest,TResponse>` for FluentValidation
- Error handling:
  - `ExceptionHandlingMiddleware` maps exceptions to consistent HTTP ProblemDetails

## Folder Structure
```
productOrderApi/
├── src/
│   ├── Domain/
│   │   ├── Entities/ (BaseEntity, Product, Order, OrderItem)
│   │   ├── ValueObjects/ (Money, Address)
│   │   ├── Exceptions/ (NotFoundException, InsufficientStockException, etc.)
│   │   ├── Enums/, Events/
│   │   └── ProductOrderAPI.Domain.csproj
│   ├── Application/
│   │   ├── Products/
│   │   │   ├── Commands/ (CreateProduct, UpdateProduct, DeleteProduct)
│   │   │   ├── Queries/ (GetProductById, GetProducts)
│   │   │   └── DTOs/ (ProductDto)
│   │   ├── Orders/ (PlaceOrder, GetOrders, GetOrderById)
│   │   ├── Common/ (Behaviors, Mappings, Models, Interfaces)
│   │   ├── DependencyInjection.cs
│   │   └── ProductOrderAPI.Application.csproj
│   ├── Infrastructure/
│   │   ├── Persistence/ (ApplicationDbContext, Configurations, Repositories, Interceptors)
│   │   ├── Migrations/ (InitialCreate, Snapshot)
│   │   ├── Services/ (CurrentUserService, DateTimeService)
│   │   ├── DependencyInjection.cs
│   │   └── ProductOrderAPI.Infrastructure.csproj
│   └── API/
│       ├── Controllers/ (ProductsController)
│       ├── Middleware/ (ExceptionHandlingMiddleware, RequestLoggingMiddleware, CorrelationIdMiddleware)
│       ├── Program.cs, ProductOrderAPI.API.csproj, Properties/
│       └── Swagger via Swashbuckle
├── tests/
│   ├── UnitTests/ (Application, Domain; common async query helpers)
│   ├── IntegrationTests/ (infra and endpoint integration)
│   └── ApiTests/
└── ProductOrderAPI.slnx
```

## Domain Model
- `Product`
  - Fields: `Id`, `Name`, `Description`, `Price` (`Money`), `StockQuantity`, `CreatedAt`, `LastModifiedAt`
  - Behavior: `Update(name, description, price, stockQuantity)` updates product and sets `LastModifiedAt`
- `Money`
  - Record with `Currency` and `Amount`

## API Endpoints
- Base route: `http://localhost:5029`
- Swagger UI: `http://localhost:5029/swagger/index.html`
- Products:
  - `POST /api/Products` — create product
  - `GET /api/Products/{id}` — get product by id
  - `PUT /api/Products/{id}` — update product
  - `DELETE /api/Products/{id}` — delete product

### Request/Response Examples
- Create product request body:
```json
{
  "name": "Laptop",
  "description": "14" Ultrabook",
  "price": { "currency": "USD", "amount": 1299.99 },
  "stockQuantity": 25
}
```
- Product response (`ProductDto`):
```json
{
  "id": "a2c43c0f-8c0f-4b3d-90f1-3b8f8b2b9e10",
  "name": "Laptop",
  "description": "14\" Ultrabook",
  "price": { "currency": "USD", "amount": 1299.99 },
  "stockQuantity": 25
}
```

## Validation & Error Handling
- FluentValidation rules enforced for create/update requests (e.g., name and description required, price amount > 0, currency length = 3, stock quantity >= 0)
- Errors return `ProblemDetails` JSON:
  - `400 Bad Request` for validation errors
  - `404 Not Found` when entity not found
  - `500 Internal Server Error` for unhandled exceptions

## Persistence
- EF Core with PostgreSQL via Npgsql
- `ApplicationDbContext` exposes `DbSet<Product>` and applies configurations from assembly
- Connection string: resolved via `configuration.GetConnectionString("Default")`
  - Provide via `appsettings.json`, environment variable, or user secrets
  - Example `appsettings.json` snippet:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=productorderapi;Username=postgres;Password=yourpassword"
  }
}
```

## Running Locally
- Prerequisites:
  - .NET 8 SDK
  - PostgreSQL running locally
- Setup:
  - Configure connection string named `Default` as above
  - Apply migrations: from `src/Infrastructure` or solution root
    - `dotnet ef database update --project src/Infrastructure/ProductOrderAPI.Infrastructure.csproj --startup-project src/API/ProductOrderAPI.API.csproj`
- Run API:
  - `dotnet run --project src/API/ProductOrderAPI.API.csproj`
  - Navigate to `http://localhost:5029/swagger/index.html`

## Clone & Setup
- Clone the repository:
  - `git clone https://github.com/your-org/productOrderApi.git`
  - `cd productOrderApi`
- Configure database:
  - Add a `ConnectionStrings:Default` entry (appsettings or environment variable)
  - Example environment variable on Windows PowerShell:
    - `$env:ConnectionStrings__Default = "Host=localhost;Port=5432;Database=productorderapi;Username=postgres;Password=yourpassword"`
- Apply migrations:
  - `dotnet ef database update --project src/Infrastructure/ProductOrderAPI.Infrastructure.csproj --startup-project src/API/ProductOrderAPI.API.csproj`
- Run the API:
  - `dotnet run --project src/API/ProductOrderAPI.API.csproj`
  - Open Swagger: `http://localhost:5029/swagger/index.html`

## Testing
- Unit tests path: `tests/UnitTests`
- Run all tests:
  - `dotnet test tests/UnitTests/ProductOrderAPI.UnitTests.csproj`
- Key coverage:
  - Command handlers: create, update, delete product
  - NotFound scenarios and validation pipeline

## Why This Architecture
- Onion Architecture benefits:
  - Domain-centric: business rules stay pure and independent of frameworks
  - Clear boundaries: outer layers depend inward, never the reverse
  - Flexibility: swap infrastructure (e.g., database provider) without touching use cases
  - Testability: application logic tested in isolation via interfaces and mocks
- CQRS rationale:
  - Explicit intent: separates reads and writes for clarity and maintenance
  - Evolvability: supports scaling reads/writes independently and advanced patterns later
  - Simpler validation and error handling pipelines per request type

## Extending the API
- Add a new feature by creating:
  - Domain changes if needed (entity/value object)
  - Application command/query + handler + validator + DTO mapping
  - Infrastructure persistence/config updates
  - API controller action and Swagger is updated automatically
- Example: add list products query
  - `GetProductsQuery` + handler in `src/Application/Products/Queries/GetProducts`
  - Controller action `[HttpGet]` in `ProductsController`

## Notes
- Swagger shows `DELETE /api/Products/{id}`; actual status returned is `204 No Content` for success and `404 Not Found` for missing products.
- Serilog packages are referenced; integrate logging in `Program.cs` if you want structured logs to console/file.
