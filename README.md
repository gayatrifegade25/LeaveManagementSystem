# Leave Management System

A full-stack employee leave management app: employees submit leave requests, managers approve or reject them. Built to demonstrate an end-to-end .NET + Angular workflow with authentication, a relational database, automated tests, and CI.

## Tech stack

| Layer | Tech |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core (code-first) |
| Database | SQL Server |
| Auth | JWT bearer tokens, role-based (`Employee` / `Manager`) |
| Frontend | Angular (standalone components, reactive forms) |
| Testing | xUnit |
| CI | GitHub Actions |
| Docs | Swagger / OpenAPI |

## Features

- Register / login with hashed passwords (BCrypt) and JWT issuance
- Employees can submit, view, and cancel their own leave requests
- Managers can view all requests and approve/reject with a comment
- Leave balance is validated and deducted automatically on approval
- Role-based authorization enforced at the API level (`[Authorize(Roles = "Manager")]`)
- Swagger UI for exploring and testing the API directly

## Architecture

```
backend/
  LeaveManagement.API/     -> Web API: controllers, models, EF Core, JWT auth, Swagger
  LeaveManagement.Tests/   -> xUnit tests for core business logic
frontend/
  src/app/
    services/              -> AuthService, LeaveService, JWT interceptor
    components/             -> leave-list, leave-form
.github/workflows/ci.yml   -> build + test on every push/PR
```

Business rules (date validation, leave balance checks) are pulled out of the controller into `LeaveRequestService`, specifically so they can be unit tested without spinning up a database — this mirrors how you'd structure a real production codebase for testability.

## Design decisions

- **JWT over cookies**: keeps the API stateless and works cleanly with an Angular SPA calling it from a different origin.
- **SQL Server**: matches an enterprise .NET environment rather than defaulting to SQLite, since that's the target stack.
- **DTOs instead of exposing EF entities directly**: avoids leaking internal fields (like `PasswordHash`) through API responses and decouples the API contract from the database schema.
- **Service layer for business rules**: keeps controllers thin and makes the core logic (leave balance checks, date validation) unit-testable in isolation.

## Getting started

### Backend

```bash
cd backend/LeaveManagement.API
dotnet restore
dotnet ef database update    # requires dotnet-ef tool: dotnet tool install --global dotnet-ef
dotnet run
```

API will be available at `https://localhost:5001`, with Swagger UI at `https://localhost:5001/swagger`.

Update the `Jwt:Key` and `ConnectionStrings:DefaultConnection` values in `appsettings.json` before running — do not commit real secrets.

### Frontend

The `frontend/src/app` folder contains the core Angular pieces (services, interceptor, components). To run them, scaffold a new Angular project and drop these files in:

```bash
ng new leave-app --routing --style=css
cd leave-app
# copy the contents of frontend/src/app into src/app
ng serve
```

Register the interceptor in `app.config.ts`:

```typescript
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './services/auth.interceptor';

providers: [
  provideHttpClient(withInterceptors([authInterceptor]))
]
```

### Tests

```bash
cd backend/LeaveManagement.Tests
dotnet test
```

## Possible next steps

- Add integration tests using `WebApplicationFactory` against an in-memory database
- Add pagination and filtering on `GET /api/leaverequests`
- Add a manager dashboard view in Angular for reviewing pending requests
- Deploy backend to Azure App Service and frontend to Azure Static Web Apps
