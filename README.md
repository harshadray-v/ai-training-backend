# Customer API

A .NET 9 Web API that provides RESTful CRUD operations for managing customers. Built with ASP.NET Core controllers, Entity Framework Core, and SQL Server.

## Architecture

```
CustomerApi/
├── CustomerApi.sln                    # Solution file
├── CustomerApi.Api/                   # Web API project (entry point)
│   ├── Program.cs                     # DI, CORS, Swagger, middleware
│   ├── Controllers/
│   │   └── CustomersController.cs     # CRUD endpoints for customers
│   ├── appsettings.json               # Configuration (DB connection, CORS)
│   └── Properties/launchSettings.json # Dev server ports
├── CustomerApi.Core/                  # Class library (entities, data, repos)
│   ├── Entities/Customer.cs           # Customer entity
│   ├── DTOs/                          # Data Transfer Objects
│   │   ├── CustomerDto.cs             # Read DTO
│   │   ├── CreateCustomerDto.cs       # Create DTO with validation
│   │   └── UpdateCustomerDto.cs       # Update DTO (partial, nullable fields)
│   ├── Data/CustomerDbContext.cs       # EF Core DbContext + seed data
│   ├── Repositories/
│   │   ├── ICustomerRepository.cs     # Repository interface
│   │   └── CustomerRepository.cs      # EF Core implementation
│   └── Migrations/                    # EF Core migrations
└── CustomerApi.Tests/                 # xUnit test project
    ├── CustomerRepositoryTests.cs     # Unit tests (EF InMemory)
    ├── CustomerEndpointsTests.cs      # Integration tests (WebApplicationFactory)
    └── DtoValidationTests.cs          # DTO validation tests
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or any SQL Server instance)
- [EF Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)

## Setup & Run

### 1. Configure Database Connection

Edit `CustomerApi.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CustomerDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Adjust the `Server` value if your SQL Server instance is different (e.g., `(localdb)\\mssqllocaldb`, `.`, `localhost`).

### 2. Start SQL Server

Ensure SQL Server Express is running:

```powershell
# Check status
Get-Service -Name "MSSQL$SQLEXPRESS"

# Start if stopped (requires admin)
Start-Service -Name "MSSQL$SQLEXPRESS"
```

### 3. Apply Database Migration

The API automatically applies pending migrations on startup in Development mode. Alternatively, run manually:

```bash
cd CustomerApi
dotnet ef database update --startup-project CustomerApi.Api
```

This creates the `CustomerDb` database and seeds it with 12 sample customers.

### 4. Run the API

```bash
cd CustomerApi/CustomerApi.Api
dotnet run
```

The API starts at:
- **HTTP**: `http://localhost:5120`
- **HTTPS**: `https://localhost:7117`
- **Swagger UI**: `http://localhost:5120/swagger`

## API Endpoints

Base URL: `/api/customers`

| Method   | Route                 | Description              | Request Body        | Response                  |
|----------|-----------------------|--------------------------|---------------------|---------------------------|
| `GET`    | `/api/customers`      | List all customers       | —                   | `200 OK` — `CustomerDto[]` |
| `GET`    | `/api/customers/{id}` | Get customer by ID       | —                   | `200 OK` / `404 Not Found` |
| `POST`   | `/api/customers`      | Create a new customer    | `CreateCustomerDto` | `201 Created`             |
| `PUT`    | `/api/customers/{id}` | Update existing customer | `UpdateCustomerDto` | `200 OK` / `404 Not Found` |
| `DELETE` | `/api/customers/{id}` | Delete a customer        | —                   | `204 No Content` / `404`  |

### Request/Response Models

**CustomerDto** (Response):
```json
{
  "id": 1,
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice.johnson@acme.com",
  "phone": "(555) 100-1001",
  "company": "Acme Corp",
  "status": "active",
  "createdAt": "2024-01-15T00:00:00Z"
}
```

**CreateCustomerDto** (POST body):
```json
{
  "firstName": "John",       // required, min 2 chars
  "lastName": "Doe",         // required, min 2 chars
  "email": "john@acme.com",  // required, valid email
  "phone": "(555) 123-4567", // required
  "company": "ACME Corp",    // required
  "status": "active"         // required, "active" or "inactive"
}
```

**UpdateCustomerDto** (PUT body — all fields optional):
```json
{
  "firstName": "Jane",
  "company": "New Corp"
}
```

### Validation Rules

| Field       | Create    | Update    | Rules                                          |
|-------------|-----------|-----------|------------------------------------------------|
| `firstName` | Required  | Optional  | Min 2 chars, max 100 chars                     |
| `lastName`  | Required  | Optional  | Min 2 chars, max 100 chars                     |
| `email`     | Required  | Optional  | Valid email format, max 255 chars              |
| `phone`     | Required  | Optional  | Max 50 chars                                   |
| `company`   | Required  | Optional  | Max 200 chars                                  |
| `status`    | Required  | Optional  | Must be `"active"` or `"inactive"`             |

Invalid requests return `400 Bad Request` with a validation problem details response.

## CORS Configuration

The API allows cross-origin requests from the Angular development server. Configured in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  }
}
```

## Running Tests

```bash
cd CustomerApi
dotnet test --verbosity normal
```

**Test coverage (32 tests):**

- **CustomerRepositoryTests** (11 tests) — Unit tests for repository CRUD using EF Core InMemory
- **CustomerEndpointsTests** (10 tests) — Integration tests for all API endpoints using `WebApplicationFactory`
- **DtoValidationTests** (11 tests) — Validation attribute tests for Create/Update DTOs

## Database Schema

The `Customers` table is created by EF Core migrations:

| Column      | Type           | Constraints                    |
|-------------|----------------|--------------------------------|
| `Id`        | `int`          | PK, Identity, auto-increment  |
| `FirstName` | `nvarchar(100)` | NOT NULL                      |
| `LastName`  | `nvarchar(100)` | NOT NULL                      |
| `Email`     | `nvarchar(255)` | NOT NULL, Unique Index         |
| `Phone`     | `nvarchar(50)`  | NOT NULL                      |
| `Company`   | `nvarchar(200)` | NOT NULL                      |
| `Status`    | `nvarchar(20)`  | NOT NULL, Default: `'active'` |
| `CreatedAt` | `datetime2`    | NOT NULL                       |

### Seed Data

12 sample customers are seeded automatically (matching the Angular mock data):

| ID | Name             | Company            | Status   |
|----|------------------|--------------------|----------|
| 1  | Alice Johnson    | Acme Corp          | active   |
| 2  | Bob Smith        | Globex Inc         | active   |
| 3  | Carol Williams   | Initech            | inactive |
| 4  | David Brown      | Umbrella LLC       | active   |
| 5  | Eva Davis        | Wayne Enterprises  | active   |
| 6  | Frank Garcia     | Stark Industries   | inactive |
| 7  | Grace Martinez   | Oscorp             | active   |
| 8  | Henry Rodriguez  | LexCorp            | active   |
| 9  | Irene Wilson     | Cyberdyne Systems  | inactive |
| 10 | James Anderson   | Weyland-Yutani     | active   |
| 11 | Karen Thomas     | Soylent Corp       | active   |
| 12 | Leo Jackson      | Massive Dynamic    | inactive |

## Angular Integration

The Angular app at `http://localhost:4200` communicates with this API. The Angular `CustomerService` is configured to call `http://localhost:5120/api/customers`.

To run both together:

```bash
# Terminal 1 — Start API
cd CustomerApi/CustomerApi.Api
dotnet run

# Terminal 2 — Start Angular
cd ai-training-1
ng serve
```
