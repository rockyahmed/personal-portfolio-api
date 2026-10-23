# User Management API (.NET Core - Clean Architecture)

A modern, production-ready **User Management REST API** built with **ASP.NET Core (.NET 10)** following the **Clean Architecture** pattern. It features JWT Bearer Authentication, Role-based Access Control (RBAC), SQLite persistence via Entity Framework Core, structured error handling, and interactive Swagger UI documentation.

---

## 🏛️ Architecture Overview

The solution follows clean separation of concerns across 4 distinct layers:

```
src/
├── UserManagement.Domain/          # Core business entities, enums, domain exceptions
│   ├── Entities/
│   │   └── User.cs                 # User entity with domain logic & soft delete
│   ├── Enums/
│   │   ├── UserRole.cs             # Admin, Manager, User
│   │   └── UserStatus.cs           # Active, Inactive, Suspended, PendingVerification
│   └── Exceptions/                 # Domain-specific exceptions
│
├── UserManagement.Application/     # Contracts, DTOs, and application models
│   ├── Common/
│   │   ├── ApiResponse.cs          # Standard API response envelope
│   │   ├── PagedResult.cs          # Generic pagination result metadata
│   │   └── PaginationFilter.cs     # Search, filter, and pagination parameters
│   ├── DTOs/
│   │   ├── Auth/                   # Login, Register, RefreshToken payloads
│   │   └── User/                   # User CRUD and profile request/response DTOs
│   └── Interfaces/                 # Repository and Service contracts
│
├── UserManagement.Infrastructure/  # EF Core SQLite, security & service implementations
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs # EF Core DbContext with soft delete filters & indexes
│   │   └── UserDbContextSeed.cs    # Out-of-the-box admin & sample user seeder
│   ├── Repositories/
│   │   └── UserRepository.cs       # EF Core data access repository
│   ├── Security/
│   │   ├── PasswordHasher.cs       # Secure BCrypt password hashing
│   │   ├── JwtSettings.cs          # Token parameters
│   │   └── TokenService.cs         # JWT token generator & validator
│   └── Services/
│       ├── UserService.cs          # User CRUD business rules
│       └── AuthService.cs          # Authentication and token refresh workflow
│
└── UserManagement.Api/             # Presentation layer, controllers & pipeline
    ├── Configurations/
    │   └── SwaggerConfig.cs        # Swagger UI, JWT Bearer definition & XML docs
    ├── Controllers/
    │   ├── BaseApiController.cs    # Shared base controller with ApiResponse helpers
    │   ├── AuthController.cs       # Register, Login, Refresh, Me, Revoke
    │   └── UsersController.cs      # User management CRUD & status/role updates
    ├── Middleware/
    │   └── GlobalExceptionHandlerMiddleware.cs # Uniform JSON error formatting
    ├── Program.cs                  # Pipeline setup & startup seed execution
    └── appsettings.json            # Database & JWT configurations
```

---

## ⚡ Key Features

- **Clean Architecture & SOLID Principles**: Decoupled domain, application, infrastructure, and presentation layers.
- **Interactive Swagger View**: Built-in Swashbuckle OpenAPI with XML documentation summaries, parameter guides, response types, and direct JWT Bearer token authentication button (**Authorize** lock button).
- **Authentication & JWT**:
  - HMAC-SHA256 access tokens.
  - Cryptographically secure refresh tokens with automatic token rotation and revocation.
- **Role-Based Access Control (RBAC)**:
  - `Admin`: Full user creation, role assignment, status changes, and soft delete.
  - `Manager`: Elevated user viewing and listing permissions.
  - `User`: Self profile inspection and updates.
- **Entity Framework Core with SQLite**:
  - Zero-configuration setup; creates and seeds `usermanagement.db` automatically on first start.
  - Indexed queries for `Email` and `Username`.
  - Global query filter for transparent soft delete (`IsDeleted`).
- **Pagination, Search & Sorting**:
  - Free-text search matching username, email, first name, and last name.
  - Role and status filtering.
  - Dynamic sorting by property with ascending/descending control.
- **Consistent API Response Wrapper**:
  All endpoints return standard payloads:
  ```json
  {
    "success": true,
    "message": "Operation completed successfully.",
    "data": { ... },
    "errors": null,
    "statusCode": 200,
    "timestamp": "2026-10-23T14:30:00Z"
  }
  ```
- **Global Exception Handling**: Converts exceptions (`NotFoundException`, `ConflictException`, `ValidationException`) into appropriate HTTP status codes and unified JSON responses.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or .NET 8 / 9 runtime)

### Running the Project

From the project root directory, run:

```bash
dotnet build
dotnet run --project src/UserManagement.Api/UserManagement.Api.csproj --launch-profile http
```

The API will start at:
- **API Base URL**: `http://localhost:5281`
- **Swagger Documentation**: `http://localhost:5281/swagger`
- *Navigating to `http://localhost:5281/` automatically redirects you to Swagger UI!*

---

## 👥 Default Seeded Accounts

The database is automatically provisioned on the first application launch with these pre-configured accounts:

| Role | Username | Email | Password |
|---|---|---|---|
| **Admin** | `admin` | `admin@example.com` | `Admin@123456` |
| **Manager** | `manager` | `manager@example.com` | `Manager@123456` |
| **User** | `johndoe` | `john.doe@example.com` | `User@123456` |

---

## 📖 API Endpoints Reference

### 🔐 Authentication (`/api/v1/auth`)

| Method | Endpoint | Access | Description |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Anonymous | Register a new user account |
| `POST` | `/api/v1/auth/login` | Anonymous | Login with username/email and password to receive JWT |
| `POST` | `/api/v1/auth/refresh-token` | Anonymous | Exchange expired access token with valid refresh token |
| `GET` | `/api/v1/auth/me` | Authenticated | Retrieve current user profile |
| `POST` | `/api/v1/auth/revoke-token` | Authenticated | Revoke refresh token (Logout) |

### 👤 User Management (`/api/v1/users`)

| Method | Endpoint | Access | Description |
|---|---|---|---|
| `GET` | `/api/v1/users` | Admin, Manager | Get paginated, searchable, and sortable list of users |
| `GET` | `/api/v1/users/{id}` | Admin, Manager, Self | Get user details by ID |
| `POST` | `/api/v1/users` | Admin | Create user with specified role and status |
| `PUT` | `/api/v1/users/{id}` | Admin, Self | Update user profile (name, phone) |
| `PATCH` | `/api/v1/users/{id}/status` | Admin | Update user status (`Active`, `Inactive`, `Suspended`) |
| `PATCH` | `/api/v1/users/{id}/role` | Admin | Change user role (`Admin`, `Manager`, `User`) |
| `POST` | `/api/v1/users/{id}/change-password` | Admin, Self | Change user account password |
| `DELETE` | `/api/v1/users/{id}` | Admin | Soft-delete user account |

### 🎨 Portfolio Projects (`/api/v1/projects`)

| Method | Endpoint | Access | Description |
|---|---|---|---|
| `GET` | `/api/v1/projects` | Anonymous | Get all projects with tag filtering, search, and pagination |
| `GET` | `/api/v1/projects/featured` | Anonymous | Get featured showcase projects |
| `GET` | `/api/v1/projects/{id}` | Anonymous | Get single project details by ID |
| `POST` | `/api/v1/projects` | Admin, Manager | Create a new project (Title, Description, Image, Link, Tags) |
| `PUT` | `/api/v1/projects/{id}` | Admin, Manager | Update project details |
| `DELETE` | `/api/v1/projects/{id}` | Admin | Soft-delete a project |

---

## 🧪 Testing with `api-tests.http`

The workspace includes a ready-to-run [api-tests.http](file:///d:/Learning/Backend/personal-portfolio-api/api-tests.http) file. You can open and execute these requests directly using VS Code REST Client or Antigravity IDE.

---

## 🔒 Security Practices Implemented
- Passwords hashed with **BCrypt** with high work factor.
- Database queries use **EF Core parameterized queries** to prevent SQL injection.
- Passwords are never returned in any response DTO (`UserDto`).
- Enforced password complexity: minimum 8 characters, at least 1 uppercase, 1 lowercase, 1 number, and 1 special symbol.
- Global soft deletion prevents accidental permanent data destruction.
