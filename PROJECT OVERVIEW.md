# VSTEP Writing System

## Clean Architecture Documentation

---

## Table of Contents

* Project Overview
* Architecture Overview
* Project Structure
* Technology Stack
* Layers Description
* Database Schema
* API Endpoints
* Authentication & Authorization
* Configuration
* Development Notes

---

## Project Overview

The **VSTEP Writing System** is a comprehensive platform designed to help students practice and improve their writing skills for the **VSTEP English proficiency exam**.

The application follows **Clean Architecture principles** to ensure:

* Maintainability
* Scalability
* Testability

### Features

* User authentication and authorization
* Practice modes and sessions
* Exam structure management
* Topic and part management
* Submission tracking and review
* Role-based access control

---

## Architecture Overview

The system is designed using **Clean Architecture**, separating concerns into distinct layers:

```
┌─────────────────┐    ┌─────────────────────┐    ┌──────────────────┐
│ Presentation    │    │ Application         │    │ Domain           │
│ (API)           │◄──►│ (Business Logic)    │◄──►│ (Entities)       │
└─────────────────┘    └─────────────────────┘    └──────────────────┘
         ▲                       ▲                        ▲
         │                       │                        │
         └───────────────────────┴────────────────────────┘
                           Infrastructure
                    (Data Persistence, External Services)
```

---

## Project Structure

```
VSTEP_Writing_System/
├── src/
│   ├── API/                          # Presentation Layer
│   │   ├── Controllers/
│   │   ├── Properties/
│   │   ├── API.http
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── API.csproj
│   ├── Application/                  # Application Layer
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   ├── Mappings/
│   │   └── Application.csproj
│   ├── Domain/                       # Domain Layer
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── ValueObjects/
│   │   └── Domain.csproj
│   ├── Infrastructure/               # Infrastructure Layer
│   │   ├── Data/
│   │   ├── Repositories/
│   │   ├── DependencyInjection.cs
│   │   └── Infrastructure.csproj
├── scripts/
│   ├── add_password_reset_tokens.sql
│   └── vstep_writing_3nf.sql
├── README.md
└── VSTEP_Writing_System.sln
```

---

## Technology Stack

### Core Technologies

* .NET 8
* ASP.NET Core 8
* Entity Framework Core 8
* MySQL (Pomelo Connector)
* JWT Bearer Authentication
* BCrypt
* AutoMapper
* FluentValidation

### Architecture Patterns

* Clean Architecture
* Repository Pattern
* Dependency Injection
* CQRS

---

## Layers Description

### Domain Layer

Pure business logic without dependencies.

**Entities**

* User
* ExamStructure
* Part
* PartType
* Topic
* PracticeSession
* UserSubmission
* Level
* PracticeMode
* PasswordResetToken

**Enums**

* Role: Guest, User, Manager, Admin

---

### Application Layer

Contains use cases, DTOs, and interfaces.

**Services**

* AuthService
* JwtService
* UserService

---

### Infrastructure Layer

Responsible for database access and external services.

* EF Core DbContext
* Fluent API configurations
* Repository implementations

---

### API Layer

Handles HTTP requests and responses.

**Controllers**

* AuthController
* UsersController
* HealthController
* ExamStructuresController
* PartsController
* TopicsController
* PracticeSessionsController
* UserSubmissionsController
* LevelsController
* PartTypesController
* PracticeModesController

---

## Database Schema

The database follows **Third Normal Form (3NF)**.

### Core Tables

* users
* exam_structures
* parts
* part_types
* topics
* practice_sessions
* user_submissions
* levels
* practice_modes
* password_reset_tokens

---

## API Endpoints

### Authentication

* POST /api/auth/login
* POST /api/auth/register
* POST /api/auth/forgot-password
* POST /api/auth/reset-password
* POST /api/auth/change-password
* POST /api/auth/validate

### Users

* GET /api/users/profile
* PUT /api/users/profile

### Health Check

* GET /api/health

---

## Authentication & Authorization

### JWT Authentication

* Algorithm: HS256
* Expiration: 24 hours
* Claims: UserId, Username, Role, Email

### Authorization Policies

* AdminOnly
* ManagerOrAdmin
* UserOrAbove
* Authenticated

---

## Configuration

### Connection Strings

* DefaultConnection (MySQL)

### JWT Settings

* SecretKey
* Issuer
* Audience

### CORS

* AllowFrontend: [http://localhost:3000](http://localhost:3000)

---

## Development Notes

### Running the Application

```
dotnet run --project src/API/API.csproj
```

### Build Solution

```
dotnet build VSTEP_Writing_System.sln
```

### Notes

* Ensure MySQL is running
* Update connection string in appsettings.json
* Run database migration scripts
* Swagger enabled in Development environment

