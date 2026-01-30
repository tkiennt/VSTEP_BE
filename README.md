# VSTEP Writing System - Backend

A comprehensive backend API built with ASP.NET Core 8.0 following Clean Architecture principles, designed to help students practice and improve their writing skills for the VSTEP English proficiency exam.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Authentication & Authorization](#authentication--authorization)
- [API Endpoints](#api-endpoints)
- [Configuration](#configuration)
- [Development Notes](#development-notes)

---

## Overview

### Features

- User authentication and authorization with JWT
- Practice modes and sessions management
- Exam structure and content management
- Topic and part organization
- Submission tracking and review system
- Role-based access control (RBAC)
- Password reset functionality
- Health check endpoints

### Key Highlights

- **Clean Architecture** for maintainability and scalability
- **Third Normal Form (3NF)** database design
- **JWT Bearer** authentication with BCrypt password hashing
- **Repository Pattern** for data access abstraction
- **Dependency Injection** for loose coupling
- **Swagger/OpenAPI** documentation

---

## Architecture

The application follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────┐    ┌─────────────────────┐    ┌──────────────────┐
│   Presentation  │    │   Application       │    │   Domain         │
│   (API Layer)   │◄──►│   (Use Cases)       │◄──►│   (Entities)     │
└─────────────────┘    └─────────────────────┘    └──────────────────┘
         ▲                       ▲                        ▲
         │                       │                        │
         └───────────────────────┴────────────────────────┘
                           Infrastructure
                    (Data Access, External Services)
```

### Layer Responsibilities

- **Domain Layer**: Pure business entities and enums (no external dependencies)
- **Application Layer**: Business logic, interfaces, DTOs, and service implementations
- **Infrastructure Layer**: Data persistence (EF Core), repositories, external integrations
- **API Layer**: Controllers, HTTP request/response handling, middleware configuration

---

## Project Structure

```
VSTEP_Writing_System/
├── src/
│   ├── API/                                # Presentation Layer
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs           # Authentication endpoints
│   │   │   ├── UserController.cs           # User management
│   │   │   ├── HealthController.cs         # Health checks
│   │   │   ├── ExamStructuresController.cs
│   │   │   ├── PartsController.cs
│   │   │   ├── TopicsController.cs
│   │   │   ├── PracticeSessionsController.cs
│   │   │   ├── UserSubmissionsController.cs
│   │   │   ├── LevelsController.cs
│   │   │   ├── PartTypesController.cs
│   │   │   └── PracticeModesController.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── API.http
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── API.csproj
│   │
│   ├── Application/                        # Application Layer
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   ├── RegisterRequest.cs
│   │   │   │   ├── AuthResponse.cs
│   │   │   │   ├── ForgotPasswordRequest.cs
│   │   │   │   ├── ResetPasswordRequest.cs
│   │   │   │   └── ChangePasswordRequest.cs
│   │   │   └── User/
│   │   │       ├── UserProfileResponse.cs
│   │   │       └── UpdateProfileRequest.cs
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── Repositories/               # Repository interfaces
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   ├── IPasswordResetTokenRepository.cs
│   │   │   │   ├── IExamStructureRepository.cs
│   │   │   │   ├── IPartRepository.cs
│   │   │   │   ├── IPartTypeRepository.cs
│   │   │   │   ├── ITopicRepository.cs
│   │   │   │   ├── IPracticeSessionRepository.cs
│   │   │   │   ├── IUserSubmissionRepository.cs
│   │   │   │   ├── ILevelRepository.cs
│   │   │   │   └── IPracticeModeRepository.cs
│   │   │   │
│   │   │   └── Services/                   # Service interfaces
│   │   │       ├── IAuthService.cs
│   │   │       ├── IJwtService.cs
│   │   │       └── IUserService.cs
│   │   │
│   │   ├── Services/                       # Service implementations
│   │   │   ├── AuthService.cs
│   │   │   ├── JwtService.cs
│   │   │   └── UserService.cs
│   │   │
│   │   ├── Mappings/                       # AutoMapper profiles
│   │   └── Application.csproj
│   │
│   ├── Domain/                             # Domain Layer (Pure)
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── PasswordResetToken.cs
│   │   │   ├── ExamStructure.cs
│   │   │   ├── Part.cs
│   │   │   ├── PartType.cs
│   │   │   ├── Topic.cs
│   │   │   ├── PracticeSession.cs
│   │   │   ├── UserSubmission.cs
│   │   │   ├── Level.cs
│   │   │   └── PracticeMode.cs
│   │   │
│   │   ├── Enums/
│   │   │   └── Role.cs                     # Guest, User, Manager, Admin
│   │   │
│   │   └── Domain.csproj
│   │
│   └── Infrastructure/                     # Infrastructure Layer
│       ├── Data/
│       │   ├── DbContexts/
│       │   │   └── ApplicationDbContext.cs # EF Core DbContext
│       │   │
│       │   ├── Configurations/             # Fluent API Configurations
│       │   │   ├── UserConfiguration.cs
│       │   │   ├── PasswordResetTokenConfiguration.cs
│       │   │   ├── ExamStructureConfiguration.cs
│       │   │   ├── PartConfiguration.cs
│       │   │   ├── PartTypeConfiguration.cs
│       │   │   ├── TopicConfiguration.cs
│       │   │   ├── PracticeSessionConfiguration.cs
│       │   │   ├── UserSubmissionConfiguration.cs
│       │   │   ├── LevelConfiguration.cs
│       │   │   └── PracticeModeConfiguration.cs
│       │   │
│       │   └── Migrations/                 # (Optional) EF Migrations
│       │
│       ├── Repositories/                   # Repository implementations
│       │   ├── UserRepository.cs
│       │   ├── PasswordResetTokenRepository.cs
│       │   ├── ExamStructureRepository.cs
│       │   ├── PartRepository.cs
│       │   ├── PartTypeRepository.cs
│       │   ├── TopicRepository.cs
│       │   ├── PracticeSessionRepository.cs
│       │   ├── UserSubmissionRepository.cs
│       │   ├── LevelRepository.cs
│       │   └── PracticeModeRepository.cs
│       │
│       ├── DependencyInjection.cs          # Infrastructure service registration
│       └── Infrastructure.csproj
│
├── scripts/
│   ├── vstep_writing_3nf.sql               # Main database schema
│   └── add_password_reset_tokens.sql       # Password reset table migration
│
├── .gitignore
├── README.md
└── VSTEP_Writing_System.sln
```

---

## Technology Stack

### Core Technologies

- **.NET 8** - Runtime environment
- **ASP.NET Core 8** - Web framework
- **Entity Framework Core 8** - ORM
- **MySQL** - Relational database
- **Pomelo.EntityFrameworkCore.MySql** - MySQL provider for EF Core

### Security & Authentication

- **JWT Bearer** - Token-based authentication
- **BCrypt.Net-Next** - Password hashing with salt

### Other Libraries

- **Swashbuckle.AspNetCore** - Swagger/OpenAPI documentation
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation (optional)

### Architecture Patterns

- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling
- **CQRS** - Command Query Responsibility Segregation (optional)

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (version 8.0 or higher)
- A code editor (Visual Studio 2022, VS Code, or Rider)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Backend
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Set up the database** (see [Database Setup](#database-setup) section)

4. **Update connection string** in `src/API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Port=3306;Database=vstep_writing;Uid=root;Pwd=YOUR_PASSWORD;CharSet=utf8mb4;"
   }
   ```

5. **Build the solution**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run --project src/API/API.csproj
   ```

7. **Access the application**
   - HTTP: `http://localhost:5268`
   - HTTPS: `https://localhost:7061`
   - Swagger UI: `http://localhost:5268/swagger`

---

## Database Setup

The project uses **MySQL** with a **Third Normal Form (3NF)** schema. The database is managed via SQL scripts (not EF Core migrations).

### 1. Create Database

Run the complete setup script in MySQL:

```bash
mysql -u root -p < Scripts/vstep_writing.sql
```

Or open `Scripts/vstep_writing.sql` in MySQL Workbench/CLI and execute the entire script.

**This script will:**
- Drop and recreate database `vstep_writing` (utf8mb4)
- Create all necessary tables with proper relationships and constraints
- Insert seed data for:
  - 6 proficiency levels (A1, A2, B1, B2, C1, C2)
  - 2 part types (Writing Part 1 & 2)
  - 5 practice modes (full test, by part, timed, untimed, random)
  - Sample hint types, prompt purposes, and sample types
  - Default admin user (username: `admin`, password: `Admin123!`)
- Users table includes: username, password_hash, role (VARCHAR), updated_at, is_active
- Password reset tokens table with 24-hour expiration

### 2. Database Schema Overview

**Core Tables:**
- `users` - User accounts with authentication data
- `password_reset_tokens` - Password reset tokens with expiration
- `levels` - Proficiency levels (A1, A2, B1, B2, C1, C2)
- `part_types` - Types of exam parts
- `practice_modes` - Different practice modes (timed, untimed, etc.)
- `exam_structures` - Exam configurations
- `parts` - Sections of exams
- `topics` - Writing topics for practice
- `practice_sessions` - User practice sessions
- `user_submissions` - User's writing submissions

**Key Relationships:**
- Users have one target level
- Users have many practice sessions
- Practice sessions belong to one user and one practice mode
- Parts belong to one exam structure and one part type
- Topics belong to one part and one difficulty level
- User submissions belong to one practice session, topic, and part

### 3. Connection String Configuration

Update your connection string in `src/API/appsettings.json` or `src/API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=vstep_writing;Uid=root;Pwd=YOUR_PASSWORD;CharSet=utf8mb4;"
  }
}
```

**Important:** Replace `YOUR_PASSWORD` with your actual MySQL password. Adjust `Server` and `Port` if using a different configuration.

---

## Authentication & Authorization

### Authentication Flow

The system uses **JWT (JSON Web Token)** for stateless authentication:

1. User logs in with username and password
2. Server validates credentials (BCrypt password verification)
3. Server generates a JWT token with user claims
4. Client includes token in `Authorization: Bearer <token>` header for subsequent requests
5. Server validates token on each protected endpoint

### Password Security

- **Hashing Algorithm**: BCrypt with auto-generated salt
- **Password Storage**: Only hashed passwords are stored (never plain text)
- **Reset Tokens**: Stored in `password_reset_tokens` table with 24-hour expiration

### User Roles

| Role | Value | Description |
|------|-------|-------------|
| **Guest** | 0 | Unauthenticated users (limited access) |
| **User** | 1 | Standard authenticated users |
| **Manager** | 2 | Elevated privileges for content management |
| **Admin** | 3 | Full system access |

### Authorization Policies

| Policy | Required Role(s) | Usage |
|--------|------------------|-------|
| `AdminOnly` | Admin | Administrative operations |
| `ManagerOrAdmin` | Manager, Admin | Content creation/modification |
| `UserOrAbove` | User, Manager, Admin | Standard user operations |
| `Authenticated` | Any authenticated user | General protected endpoints |

---

## API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Login with username/password → Returns JWT + user info | No |
| POST | `/api/auth/register` | Register new user (name, username, email, password, role) | No |
| POST | `/api/auth/validate` | Validate JWT token | Yes |
| POST | `/api/auth/forgot-password` | Request password reset (body: email) | No |
| POST | `/api/auth/reset-password` | Reset password with token (body: token, newPassword) | No |
| POST | `/api/auth/change-password` | Change password (body: currentPassword, newPassword) | Yes |

**Example Login Request:**
```json
{
  "username": "john_doe",
  "password": "SecurePassword123!"
}
```

**Example Login Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "john_doe",
  "email": "john@example.com",
  "role": "User"
}
```

---

### Health Check (`/api/health`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/ping` | Simple ping to check if API is running | No |
| GET | `/api/health` | Comprehensive health status (status, environment, database) | No |
| GET | `/api/health/db` | Database connection test (elapsed ms, user count) | No |

---

### User Management (`/api/user`)

| Method | Endpoint | Description | Auth Required | Policy |
|--------|----------|-------------|---------------|--------|
| GET | `/api/user/profile` | Get current user profile from database | Yes | Authenticated |
| PUT | `/api/user/profile` | Update profile (name, email, target_level_id) | Yes | Authenticated |
| POST | `/api/user/change-password` | Change current user password | Yes | Authenticated |
| GET | `/api/user/stats` | Get user statistics and activity summary | Yes | Authenticated |
| GET | `/api/user/practice-history` | Get user practice history | Yes | Authenticated |
| GET | `/api/user/user-only` | Test endpoint for UserOrAbove policy | Yes | UserOrAbove |
| GET | `/api/user/manager-only` | Test endpoint for ManagerOrAdmin policy | Yes | ManagerOrAdmin |
| GET | `/api/user/admin-only` | Test endpoint for AdminOnly policy | Yes | AdminOnly |

### Admin User Management (`/api/admin/users`)

| Method | Endpoint | Description | Auth Required | Policy |
|--------|----------|-------------|---------------|--------|
| GET | `/api/admin/users` | Get all users with pagination, search, and sorting | Yes | AdminOnly |
| GET | `/api/admin/users/{id}` | Get user details by ID | Yes | AdminOnly |
| POST | `/api/admin/users` | Create a new user | Yes | AdminOnly |
| PUT | `/api/admin/users/{id}` | Update user information | Yes | AdminOnly |
| PUT | `/api/admin/users/{id}/password` | Update user password | Yes | AdminOnly |
| DELETE | `/api/admin/users/{id}` | Delete a user (cannot delete admins) | Yes | AdminOnly |
| PATCH | `/api/admin/users/{id}/toggle-status` | Toggle user active status | Yes | AdminOnly |
| GET | `/api/admin/users/count` | Get total user count | Yes | AdminOnly |

#### Admin User Management Examples

**Get all users with pagination and search:**
```
GET /api/admin/users?page=1&pageSize=10&search=john&sortBy=name&sortDesc=false
```

**Create a new user:**
```json
POST /api/admin/users
{
  "name": "John Doe",
  "username": "johndoe",
  "email": "john@example.com",
  "phoneNumber": "0912345678",
  "password": "SecurePassword123!",
  "role": "User",
  "targetLevelId": 2,
  "isActive": true
}
```

**Update user information:**
```json
PUT /api/admin/users/123
{
  "name": "John Smith",
  "phoneNumber": "0987654321",
  "role": "Manager",
  "targetLevelId": 3,
  "isActive": true
}
```

**Update user password:**
```json
PUT /api/admin/users/123/password
{
  "newPassword": "NewSecurePassword456!"
}
```

**Toggle user status:**
```
PATCH /api/admin/users/123/toggle-status
```

#### User Profile Examples

**Get current user profile:**
```
GET /api/user/profile
```

**Update user profile:**
```json
PUT /api/user/profile
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "targetLevelId": 2
}
```

**Change password:**
```json
POST /api/user/change-password
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword456!"
}
```

**Get user statistics:**
```
GET /api/user/stats
```

**Get practice history:**
```
GET /api/user/practice-history?limit=20
```

---

### Lookup Endpoints (Public)

#### Levels (`/api/levels`)
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/levels` | Get all proficiency levels | No |
| GET | `/api/levels/{id}` | Get level details by ID | No |

#### Part Types (`/api/part-types`)
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/part-types` | Get all part types | No |
| GET | `/api/part-types/{id}` | Get part type details by ID | No |

#### Practice Modes (`/api/practice-modes`)
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/practice-modes` | Get all practice modes | No |
| GET | `/api/practice-modes/{id}` | Get practice mode details by ID | No |

---

### Exam & Content Management

#### Exam Structures (`/api/exam-structures`)
| Method | Endpoint | Description | Auth Required | Policy |
|--------|----------|-------------|---------------|--------|
| GET | `/api/exam-structures` | Get all exam structures | No | - |
| GET | `/api/exam-structures/{id}` | Get exam structure by ID | No | - |
| POST | `/api/exam-structures` | Create new exam structure | Yes | ManagerOrAdmin |
| PUT | `/api/exam-structures/{id}` | Update exam structure | Yes | ManagerOrAdmin |
| DELETE | `/api/exam-structures/{id}` | Delete exam structure | Yes | AdminOnly |

#### Parts (`/api/parts`)
| Method | Endpoint | Description | Auth Required | Policy |
|--------|----------|-------------|---------------|--------|
| GET | `/api/parts` | Get all parts | No | - |
| GET | `/api/parts/{id}` | Get part by ID | No | - |
| GET | `/api/parts/by-exam/{examStructureId}` | Get parts by exam structure | No | - |
| POST | `/api/parts` | Create new part | Yes | ManagerOrAdmin |
| PUT | `/api/parts/{id}` | Update part | Yes | ManagerOrAdmin |
| DELETE | `/api/parts/{id}` | Delete part | Yes | AdminOnly |

#### Topics (`/api/topics`)
| Method | Endpoint | Description | Auth Required | Policy |
|--------|----------|-------------|---------------|--------|
| GET | `/api/topics` | Get all topics | No | - |
| GET | `/api/topics/{id}` | Get topic by ID | No | - |
| GET | `/api/topics/by-part/{partId}` | Get topics by part | No | - |
| POST | `/api/topics` | Create new topic | Yes | ManagerOrAdmin |
| PUT | `/api/topics/{id}` | Update topic | Yes | ManagerOrAdmin |
| DELETE | `/api/topics/{id}` | Delete topic | Yes | AdminOnly |

---

### Practice Management

#### Practice Sessions (`/api/practice-sessions`)
| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/api/practice-sessions/my` | Get user's practice sessions | Yes | Returns sessions for authenticated user |
| GET | `/api/practice-sessions/{id}` | Get session details | Yes | Only session owner can access |
| POST | `/api/practice-sessions` | Create new session (body: modeId, isRandom) | Yes | - |
| PUT | `/api/practice-sessions/{id}` | Update session | Yes | Only session owner |
| DELETE | `/api/practice-sessions/{id}` | Delete session | Yes | Only session owner |

#### User Submissions (`/api/user-submissions`)
| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/api/user-submissions/by-session/{sessionId}` | Get submissions by session | Yes | Only session owner |
| GET | `/api/user-submissions/{id}` | Get submission details | Yes | Only submission owner |
| POST | `/api/user-submissions` | Create submission | Yes | Body: sessionId, topicId, partId, content, wordCount?, enableHint |
| PUT | `/api/user-submissions/{id}` | Update submission | Yes | Only submission owner |
| DELETE | `/api/user-submissions/{id}` | Delete submission | Yes | Only submission owner |

---

## Configuration

### JWT Settings

Configure JWT in `src/API/appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!ChangeThisInProduction!",
    "Issuer": "VSTEP_Writing_System",
    "Audience": "VSTEP_Writing_System",
    "ExpirationMinutes": "1440"
  }
}
```

**Important Security Notes:**
- **NEVER** use the default `SecretKey` in production
- Generate a secure random key (at least 32 characters)
- Store secrets in environment variables or Azure Key Vault in production
- Token expiration is set to 1440 minutes (24 hours) by default

### CORS Configuration

CORS is configured to allow requests from the frontend:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

**For production:** Update allowed origins to match your frontend domain.

### Environment-Specific Settings

- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides (not committed to Git)

---

## Development Notes

### Building the Solution

```bash
# Build all projects
dotnet build

# Build specific project
dotnet build src/API/API.csproj

# Clean and rebuild
dotnet clean
dotnet build
```

### Running the Application

```bash
# Run from API project directory
cd src/API
dotnet run

# Or from solution root
dotnet run --project src/API/API.csproj

# Watch mode (auto-reload on file changes)
dotnet watch --project src/API/API.csproj
```

### Testing the API

1. **Using Swagger UI**: Navigate to `http://localhost:5268/swagger`
2. **Using API.http**: Open `src/API/API.http` in Visual Studio/VS Code with REST Client extension
3. **Using Postman/Insomnia**: Import endpoints from Swagger JSON

### Key Design Principles

1. **Clean Architecture**: Dependencies point inward (Domain has no dependencies)
2. **Separation of Concerns**: Each layer has a single responsibility
3. **Dependency Injection**: All dependencies are injected via constructors
4. **Repository Pattern**: Data access is abstracted through repositories
5. **Entity Framework Core**: Fluent API for entity configuration (no data annotations)
6. **No EF Migrations**: Database schema is managed via SQL scripts

### Common Tasks

**Adding a new entity:**
1. Create entity in `Domain/Entities`
2. Create configuration in `Infrastructure/Data/Configurations`
3. Apply configuration in `ApplicationDbContext.OnModelCreating`
4. Update database script in `scripts/`
5. Create repository interface in `Application/Interfaces/Repositories`
6. Implement repository in `Infrastructure/Repositories`
7. Register repository in `Infrastructure/DependencyInjection.cs`

**Adding a new API endpoint:**
1. Create DTO in `Application/DTOs`
2. Create/update service interface in `Application/Interfaces/Services`
3. Implement service in `Application/Services`
4. Create/update controller in `API/Controllers`
5. Apply authorization policies as needed

### Important Notes

- **Database Management**: Schema is managed via SQL scripts, not EF migrations
- **Password Security**: Always use BCrypt for password hashing
- **JWT Tokens**: Include `Authorization: Bearer <token>` header for protected endpoints
- **Password Reset**: Tokens expire after 24 hours; in production, integrate email service
- **CORS**: Update allowed origins for production deployment
- **Logging**: Detailed logging is enabled in development environment
- **Swagger**: Available only in development by default

---

## Dependencies Summary

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.x | JWT authentication |
| Microsoft.EntityFrameworkCore | 8.x | ORM framework |
| Pomelo.EntityFrameworkCore.MySql | 8.x | MySQL provider for EF Core |
| BCrypt.Net-Next | Latest | Password hashing |
| Swashbuckle.AspNetCore | Latest | Swagger/OpenAPI documentation |
| AutoMapper | Latest | Object mapping (optional) |

---

**Last Updated**: January 2026

